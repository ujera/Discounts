using Discounts.Application;
using Discounts.Application.DTOs.Auth;
using Discounts.Application.Services;
using Discounts.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace Discounts.UnitTests
{
    public class AuthServiceTest
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private readonly AuthService _service;

        public AuthServiceTest()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null!, null!, null!, null!);

            var jwtSettings = Options.Create(new JwtSettings
            {
                Key = "super_secret_key_long_enough_for_sha256",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                DurationInMinutes = 60
            });

            _service = new AuthService(_mockUserManager.Object, _mockRoleManager.Object, jwtSettings);
        }

        #region Register Tests

        [Fact]
        public async Task RegisterAsync_WhenUserExists_ShouldThrowBadRequestException()
        {
            // Arrange
            var request = new RegisterRequest { Email = "test@test.com" };
            _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email))
                            .ReturnsAsync(new ApplicationUser());

            // Act & Assert
            await Assert.ThrowsAsync<Discounts.Application.Exceptions.BadRequestException>(() =>
                _service.RegisterAsync(request));
        }

        [Fact]
        public async Task RegisterAsync_WhenValid_ShouldCreateUserAndAssignRole()
        {
            // Arrange
            var request = new RegisterRequest { Email = "new@test.com", Password = "Password123!", Role = "Merchant" };
            _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync((ApplicationUser)null!);
            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
                            .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), request.Role))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            await _service.RegisterAsync(request);

            // Assert
            _mockUserManager.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), request.Password), Times.Once);
            _mockUserManager.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), request.Role), Times.Once);
        }

        #endregion

        #region Login Tests

        [Fact]
        public async Task LoginAsync_WhenCredentialsValid_ShouldReturnToken()
        {
            // Arrange
            var request = new LoginRequest { Email = "user@test.com", Password = "CorrectPassword" };
            var user = new ApplicationUser { Id = "1", Email = request.Email, IsActive = true };

            _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.CheckPasswordAsync(user, request.Password)).ReturnsAsync(true);
            _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Customer" });

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            Assert.NotNull(result.Token);
            Assert.Equal(request.Email, result.Email);
        }
        #endregion
    }
}
