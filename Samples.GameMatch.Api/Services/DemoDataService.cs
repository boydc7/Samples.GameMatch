using System;
using System.Collections.Generic;
using System.Linq;

namespace Samples.GameMatch.Api
{
    public class DemoDataService : IDemoDataService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IUserRatingRepository _userRatingRepository;

        public DemoDataService(IUserRepository userRepository, ISettingsRepository settingsRepository,
                               IUserRatingRepository userRatingRepository)
        {
            _userRepository = userRepository;
            _settingsRepository = settingsRepository;
            _userRatingRepository = userRatingRepository;
        }

        public void CreateDemoData()
        {
            if (!_settingsRepository.GetAll().Any())
            {
                _settingsRepository.Add(new Setting
                                        {
                                            DefaultMaxMmrGap = 0
                                        });
            }

            _userRepository.Add(new User
                                {
                                    FirstName = "Demo",
                                    LastName = "Admin",
                                    Email = "demoadmin@gamematchdemo.io",
                                    Password = "MyAdminsSuperSecretPassword".ToSha256Base64(),
                                    Role = UserRole.Admin
                                });

            var userIds = new List<Guid>(50);

            // Seed some sample users
            for (var x = 1; x <= 50; x++)
            {
                userIds.Add(_userRepository.Add(new User
                                                {
                                                    FirstName = $"Demo{x}",
                                                    LastName = $"Last{x}",
                                                    Email = $"demouser{x}@gamematchdemo.io",
                                                    Password = $"User{x}SuperSecretPassword".ToSha256Base64(),
                                                    Role = UserRole.User
                                                }));
            }

            // Seed some ratings for those users
            var rating = 0.1;

            foreach (var userId in userIds)
            {
                _userRatingRepository.Add(new UserRating
                                          {
                                              UserId = userId,
                                              GameType = GameType.Chess,
                                              Rating = rating
                                          });

                rating += .1;
            }



        }
    }
}
