using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using NaviGateway.Exceptions;
using NaviGateway.Model;
using NaviGateway.Model.Request;
using NaviGateway.Repository;

namespace NaviGateway.Service
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IKafkaIntegration _kafkaIntegration;
        private readonly IStorageIntegration _storageIntegration;

        public UserService(IUserRepository userRepository, IKafkaIntegration kafkaIntegration, IStorageIntegration storageIntegration)
        {
            _userRepository = userRepository;
            _kafkaIntegration = kafkaIntegration;
            _storageIntegration = storageIntegration;
        }

        public async Task RegisterUser(RegisterRequest request)
        {
            // Create User from Request
            var user = new User(request);
            
            // Try to save User
            try
            {
                await _userRepository.RegisterUserAsync(user);
            }
            catch (Exception superException)
            {
                HandleRegisterError(superException, user);
            }
            
            // Notify storage service to create root folder for new user.
            await _storageIntegration.RequestRootFolderCreation(user.UserEmail);
        }

        public async Task<AccessToken> LoginUser(LoginRequest request)
        {
            // Find User with Id
            var userEntity = await _userRepository.FindUserByEmailAsync(request.UserEmail);

            if (userEntity?.CheckPassword(request.UserPassword) is false or null)
            {
                throw new ApiServerException(HttpStatusCode.Forbidden, "Id or password is wrong!");
            }

            // If Matches - Create Access Token and register to user[update user db]
            var accessToken = GenerateToken(userEntity.UserEmail);
            await _userRepository.AddAccessTokenToUserAsync(userEntity.UserEmail, accessToken);

            return accessToken;
        }

        public async Task<string> AuthenticateUser(string accessToken)
        {
            // Get Authentication Access Token
            // Search it through User DB
            var userEntity = await _userRepository.FindUserByAccessTokenAsync(accessToken);

            return userEntity?.UserEmail;
        }

        public async Task RemoveUser(AccountRemovalRequest request)
        {
            // Email is verified by now
            // Remove User-Related thingy from user-db.
            await _userRepository.RemoveUserByEmailId(request.UserEmail);
            
            // Send User Removal request on Storage Service.[Queue]
            await _kafkaIntegration.SendRemovalRequest(request.UserEmail);
        }

        private AccessToken GenerateToken(string userEmail)
        {
            var concatString = $"{userEmail}/{DateTime.Now.Ticks}";
            using var shaManager = new SHA512Managed();
            var hashValue = shaManager.ComputeHash(Encoding.UTF8.GetBytes(concatString));
            
            return new AccessToken
            {
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddDays(1),
                Token = BitConverter.ToString(hashValue).Replace("-", "").ToLower()
            };
        }

        [ExcludeFromCodeCoverage]
        private Task HandleRegisterError(Exception superException, User toRegister)
        {
            // When Error type is MongoWriteException
            if (superException is MongoWriteException mongoWriteException)
            {
                // When Error Type is 'Duplicate Key'
                if (mongoWriteException.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    throw new ApiServerException(HttpStatusCode.Conflict,
                        $"User Email {toRegister.UserEmail} already exists!");
                } // Else -> goto Unknown Error.
            }

            // Unknown if exception is not MongoWriteException.
            throw new ApiServerException(HttpStatusCode.InternalServerError,
                $"Unknown Error Occurred! : {superException.Message}", superException);
        }
    }
}