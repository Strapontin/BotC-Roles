using BotcRoles.Controllers;
using BotcRoles.Enums;
using BotcRoles.Models;
using BotcRoles.Test.HelperMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotcRoles.Test
{
    [TestFixture]
    public class GameControllerShould
    {
        [Test]
        public void Post_And_Get_Game()
        {
            // Arrange
            string fileName = Helper.GetCurrentMethodName() + ".db";
            var modelContext = Helper.GetContext(fileName);
            string moduleName = "ModuleName";
            string storyTellerName = "StoryTellerName";

            GameHelper.CreateModuleAndStoryTellerForGame(modelContext, moduleName, out long moduleId, storyTellerName, out long storyTellerId);

            // Act
            var res = GameHelper.PostGame(modelContext, moduleId, storyTellerId);

            // Assert POST Ok
            Assert.AreEqual(StatusCodes.Status201Created, ((CreatedResult)res).StatusCode);

            // Act
            long gameId = GameHelper.GetGames(modelContext).First().GameId;
            var game = GameHelper.GetGame(modelContext, gameId);

            // Assert GET Ok
            Assert.AreEqual(moduleName, game.Module.Name);
            Assert.AreEqual(storyTellerName, game.StoryTeller.Name);

            Helper.DeleteCreatedDatabase(modelContext);
        }

        [Test]
        public void Cant_Post_Game_With_Wrong_PlayerId()
        {
            // Arrange
            string fileName = Helper.GetCurrentMethodName() + ".db";
            var modelContext = Helper.GetContext(fileName);
            string moduleName = "ModuleName";

            ModuleHelper.PostModule(modelContext, moduleName);
            long moduleId = ModuleHelper.GetModules(modelContext).First().ModuleId;

            // Act
            var res = GameHelper.PostGame(modelContext, moduleId, 1);

            // Assert
            Assert.AreEqual(StatusCodes.Status400BadRequest, ((BadRequestObjectResult)res).StatusCode);

            Helper.DeleteCreatedDatabase(modelContext);
        }

        [Test]
        public void Cant_Post_Game_With_Wrong_ModuleId()
        {
            // Arrange
            string fileName = Helper.GetCurrentMethodName() + ".db";
            var modelContext = Helper.GetContext(fileName);
            string playerName = "PlayerName";

            PlayerHelper.PostPlayer(modelContext, playerName);
            long storyTellerId = PlayerHelper.GetPlayers(modelContext).First().PlayerId;

            // Act
            var res = GameHelper.PostGame(modelContext, 1, storyTellerId);

            // Assert
            Assert.AreEqual(StatusCodes.Status400BadRequest, ((BadRequestObjectResult)res).StatusCode);

            Helper.DeleteCreatedDatabase(modelContext);
        }

        [Test]
        public void Can_Post_Two_Games_With_Same_StoryTellerId_And_ModuleId()
        {
            // Arrange
            string fileName = Helper.GetCurrentMethodName() + ".db";
            var modelContext = Helper.GetContext(fileName);
            string moduleName = "ModuleName";
            string storyTellerName = "StoryTellerName";

            GameHelper.CreateModuleAndStoryTellerForGame(modelContext, moduleName, out long moduleId, storyTellerName, out long storyTellerId);

            // Act
            var res = GameHelper.PostGame(modelContext, moduleId, storyTellerId);

            // Assert POST Ok
            Assert.AreEqual(StatusCodes.Status201Created, ((CreatedResult)res).StatusCode);

            // Act
            res = GameHelper.PostGame(modelContext, moduleId, storyTellerId);

            // Assert POST Ok
            Assert.AreEqual(StatusCodes.Status201Created, ((CreatedResult)res).StatusCode);

            Helper.DeleteCreatedDatabase(modelContext);
        }

        [Test]
        public void Delete_Game_Deletes_Only_One_Game()
        {
            // Arrange
            string fileName = Helper.GetCurrentMethodName() + ".db";
            var modelContext = Helper.GetContext(fileName);
            string moduleName = "ModuleName";
            string storyTellerName = "StoryTellerName";

            try
            {
                GameHelper.CreateModuleAndStoryTellerForGame(modelContext, moduleName, out long moduleId, storyTellerName, out long storyTellerId);

                GameHelper.PostGame(modelContext, moduleId, storyTellerId);
                GameHelper.PostGame(modelContext, moduleId, storyTellerId);

                var games = GameHelper.GetGames(modelContext);
                Assert.AreEqual(2, games.Count());
                long gameId = games.First().GameId;

                // Act
                var res = GameHelper.DeleteGame(modelContext, gameId);

                // Assert
                Assert.AreEqual(StatusCodes.Status200OK, ((OkResult)res).StatusCode);
                Assert.IsTrue(!GameHelper.GetGames(modelContext).Any(g => g.GameId == gameId));
            }
            finally
            {
                Helper.DeleteCreatedDatabase(modelContext);
            }
        }

        [Test]
        public void Post_And_Get_Player_In_Game()
        {
            // Arrange
            string fileName = Helper.GetCurrentMethodName() + ".db";
            var modelContext = Helper.GetContext(fileName);
            string moduleName = "ModuleName";
            string storyTellerName = "StoryTellerName";
            string playerName = "PlayerName";

            try
            {
                GameHelper.CreateModuleAndStoryTellerForGame(modelContext, moduleName, out long moduleId, storyTellerName, out long storyTellerId);
                GameHelper.PostGame(modelContext, moduleId, storyTellerId);
                var gameId = GameHelper.GetGames(modelContext).First().GameId;

                PlayerHelper.PostPlayer(modelContext, playerName);
                long playerId = PlayerHelper.GetPlayers(modelContext).First(p => p.Name == playerName).PlayerId;

                // Act
                var res = GameHelper.AddPlayerInGame(modelContext, gameId, playerId);

                // Assert
                Assert.AreEqual(StatusCodes.Status201Created, ((CreatedResult)res).StatusCode);

                // Act
                var players = GameHelper.GetPlayersInGame(modelContext, gameId)!;

                // Assert
                Assert.AreEqual(1, players.Count());
                Assert.AreEqual(playerName, players.First().Name);
            }
            finally
            {
                Helper.DeleteCreatedDatabase(modelContext);
            }
        }

        [Test]
        public void Change_Player_Role_And_Alignment_In_Game_And_Get_Correct_Value()
        {
            // Arrange
            string fileName = Helper.GetCurrentMethodName() + ".db";
            var modelContext = Helper.GetContext(fileName);
            string moduleName = "ModuleName";
            string storyTellerName = "StoryTellerName";
            string playerName = "PlayerName";
            string roleName = "RoleName";

            try
            {
                GameHelper.CreateModuleAndStoryTellerForGame(modelContext, moduleName, out long moduleId, storyTellerName, out long storyTellerId);
                GameHelper.PostGame(modelContext, moduleId, storyTellerId);
                var gameId = GameHelper.GetGames(modelContext).First().GameId;

                PlayerHelper.PostPlayer(modelContext, playerName);
                long playerId = PlayerHelper.GetPlayers(modelContext).First(p => p.Name == playerName).PlayerId;
                GameHelper.AddPlayerInGame(modelContext, gameId, playerId);

                RoleHelper.AddRole(modelContext, roleName, Enums.Type.Demon, Alignment.Evil);
                long roleId = RoleHelper.GetRoles(modelContext).First().RoleId;

                ModuleHelper.AddRoleInModule(modelContext, moduleId, roleId);

                // Act
                var res = GameHelper.ChangePlayerRoleAndAlignmentInGame(modelContext, gameId, playerId, roleId, Alignment.Evil);

                // Assert
                Assert.AreEqual(StatusCodes.Status200OK, ((OkResult)res).StatusCode);

                // Act
                var value = GameHelper.GetPlayerRoleFromGame(modelContext, gameId, playerId)!;

                // Assert
                Assert.AreEqual(1, value.Count());
                Assert.AreEqual(roleId, value.First().RoleId);
                Assert.AreEqual(Alignment.Evil, value.First().FinalAlignment);
            }
            finally
            {
                Helper.DeleteCreatedDatabase(modelContext);
            }
        }

        [Test]
        public void Cant_Change_Player_Role_And_Alignment_In_Game_If_Role_Is_Not_In_Role_Module()
        {
            // Arrange
            string fileName = Helper.GetCurrentMethodName() + ".db";
            var modelContext = Helper.GetContext(fileName);
            string moduleName = "ModuleName";
            string storyTellerName = "StoryTellerName";
            string playerName = "PlayerName";
            string roleName = "RoleName";

            try
            {
                GameHelper.CreateModuleAndStoryTellerForGame(modelContext, moduleName, out long moduleId, storyTellerName, out long storyTellerId);
                GameHelper.PostGame(modelContext, moduleId, storyTellerId);
                var gameId = GameHelper.GetGames(modelContext).First().GameId;

                PlayerHelper.PostPlayer(modelContext, playerName);
                long playerId = PlayerHelper.GetPlayers(modelContext).First(p => p.Name == playerName).PlayerId;
                GameHelper.AddPlayerInGame(modelContext, gameId, playerId);

                RoleHelper.AddRole(modelContext, roleName, Enums.Type.Demon, Alignment.Evil);
                long roleId = RoleHelper.GetRoles(modelContext).First().RoleId;

                // Act
                var res = GameHelper.ChangePlayerRoleAndAlignmentInGame(modelContext, gameId, playerId, roleId, Alignment.Evil);
                Assert.AreEqual(StatusCodes.Status400BadRequest, ((BadRequestObjectResult)res).StatusCode);
            }
            finally
            {
                Helper.DeleteCreatedDatabase(modelContext);
            }
        }

        [Test]
        public void Cant_Add_Two_PlayerRoleGame_With_Same_PlayerId_And_GameId()
        {
            // Arrange
            string fileName = Helper.GetCurrentMethodName() + ".db";
            var modelContext = Helper.GetContext(fileName);
            string moduleName = "ModuleName";
            string storyTellerName = "StoryTellerName";
            string playerName = "PlayerName";

            try
            {
                GameHelper.CreateModuleAndStoryTellerForGame(modelContext, moduleName, out long moduleId, storyTellerName, out long storyTellerId);
                GameHelper.PostGame(modelContext, moduleId, storyTellerId);
                var gameId = GameHelper.GetGames(modelContext).First().GameId;

                PlayerHelper.PostPlayer(modelContext, playerName);
                long playerId = PlayerHelper.GetPlayers(modelContext).First(p => p.Name == playerName).PlayerId;

                // Act
                var res1 = GameHelper.AddPlayerInGame(modelContext, gameId, playerId)!;
                var res2 = GameHelper.AddPlayerInGame(modelContext, gameId, playerId)!;

                // Assert
                Assert.AreEqual(StatusCodes.Status201Created, ((CreatedResult)res1).StatusCode);
                Assert.AreEqual(StatusCodes.Status400BadRequest, ((BadRequestObjectResult)res2).StatusCode);
            }
            finally
            {
                Helper.DeleteCreatedDatabase(modelContext);
            }
        }

        [Test]
        public void Return_Bad_Request_If_PlayerId_Not_Provided_When_Add_Player_In_Game()
        {
            // Arrange
            string fileName = Helper.GetCurrentMethodName() + ".db";
            var modelContext = Helper.GetContext(fileName);
            string moduleName = "ModuleName";
            string storyTellerName = "StoryTellerName";
            string playerName = "PlayerName";

            try
            {
                GameHelper.CreateModuleAndStoryTellerForGame(modelContext, moduleName, out long moduleId, storyTellerName, out long storyTellerId);
                GameHelper.PostGame(modelContext, moduleId, storyTellerId);
                var gameId = GameHelper.GetGames(modelContext).First().GameId;

                PlayerHelper.PostPlayer(modelContext, playerName);
                long playerId = PlayerHelper.GetPlayers(modelContext).First(p => p.Name == playerName).PlayerId;

                // Act
                var res = GameHelper.AddPlayerInGame(modelContext, gameId, null)!;

                // Assert
                Assert.AreEqual(StatusCodes.Status400BadRequest, ((BadRequestObjectResult)res).StatusCode);
            }
            finally
            {
                Helper.DeleteCreatedDatabase(modelContext);
            }
        }

        [Test]
        public void Return_Bad_Request_If_GameId_Not_Found_When_Add_Player_In_Game()
        {
            // Arrange
            string fileName = Helper.GetCurrentMethodName() + ".db";
            var modelContext = Helper.GetContext(fileName);
            string moduleName = "ModuleName";
            string storyTellerName = "StoryTellerName";
            string playerName = "PlayerName";

            try
            {
                GameHelper.CreateModuleAndStoryTellerForGame(modelContext, moduleName, out long moduleId, storyTellerName, out long storyTellerId);
                GameHelper.PostGame(modelContext, moduleId, storyTellerId);
                var gameId = GameHelper.GetGames(modelContext).First().GameId;

                PlayerHelper.PostPlayer(modelContext, playerName);
                long playerId = PlayerHelper.GetPlayers(modelContext).First(p => p.Name == playerName).PlayerId;

                // Act
                var res = GameHelper.AddPlayerInGame(modelContext, gameId + 1, playerId)!;

                // Assert
                Assert.AreEqual(StatusCodes.Status400BadRequest, ((BadRequestObjectResult)res).StatusCode);
            }
            finally
            {
                Helper.DeleteCreatedDatabase(modelContext);
            }
        }

        [Test]
        public void Return_Bad_Request_If_PlayerId_Not_Found_When_Add_Player_In_Game()
        {
            // Arrange
            string fileName = Helper.GetCurrentMethodName() + ".db";
            var modelContext = Helper.GetContext(fileName);
            string moduleName = "ModuleName";
            string storyTellerName = "StoryTellerName";
            string playerName = "PlayerName";

            try
            {
                GameHelper.CreateModuleAndStoryTellerForGame(modelContext, moduleName, out long moduleId, storyTellerName, out long storyTellerId);
                GameHelper.PostGame(modelContext, moduleId, storyTellerId);
                var gameId = GameHelper.GetGames(modelContext).First().GameId;

                PlayerHelper.PostPlayer(modelContext, playerName);
                long playerId = PlayerHelper.GetPlayers(modelContext).First(p => p.Name == playerName).PlayerId;

                // Act
                var res = GameHelper.AddPlayerInGame(modelContext, gameId, playerId + 1)!;

                // Assert
                Assert.AreEqual(StatusCodes.Status400BadRequest, ((BadRequestObjectResult)res).StatusCode);
            }
            finally
            {
                Helper.DeleteCreatedDatabase(modelContext);
            }
        }

        [Test]
        public void Return_Bad_Request_When_ChangePlayerRoleAndAlignmentInGame_And_PlayerRoleGame_Is_Not_Assigned_To_Game()
        {
            // Arrange
            string fileName = Helper.GetCurrentMethodName() + ".db";
            var modelContext = Helper.GetContext(fileName);
            string moduleName = "ModuleName";
            string storyTellerName = "StoryTellerName";
            string playerName = "PlayerName";
            string roleName = "RoleName";

            try
            {
                GameHelper.CreateModuleAndStoryTellerForGame(modelContext, moduleName, out long moduleId, storyTellerName, out long storyTellerId);
                GameHelper.PostGame(modelContext, moduleId, storyTellerId);
                var gameId = GameHelper.GetGames(modelContext).First().GameId;

                PlayerHelper.PostPlayer(modelContext, playerName);
                long playerId = PlayerHelper.GetPlayers(modelContext).First(p => p.Name == playerName).PlayerId;
                //GameHelper.AddPlayerInGame(modelContext, gameId, playerId);

                RoleHelper.AddRole(modelContext, roleName, Enums.Type.Demon, Alignment.Evil);
                long roleId = RoleHelper.GetRoles(modelContext).First().RoleId;

                ModuleHelper.AddRoleInModule(modelContext, moduleId, roleId);

                // Act
                var res = GameHelper.ChangePlayerRoleAndAlignmentInGame(modelContext, gameId, playerId, roleId, Alignment.Evil);

                // Assert
                Assert.AreEqual(StatusCodes.Status400BadRequest, ((BadRequestObjectResult)res).StatusCode);
            }
            finally
            {
                Helper.DeleteCreatedDatabase(modelContext);
            }
        }
    }
}
