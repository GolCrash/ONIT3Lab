using Microsoft.EntityFrameworkCore;
using Onit;
using Onit.models;
using Xunit;

public class DatabaseTests
{
    [Fact]
    public void AddUser_Should_Save_User_To_Database()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDB")
            .Options;

        using (var context = new AppDbContext(options))
        {
            var user = new User
            {
                Name = "Test",
                Email = "test@mail.com"
            };

            context.Users.Add(user);
            context.SaveChanges();
        }

        using (var context = new AppDbContext(options))
        {
            var users = context.Users.ToList();
            Assert.Single(users);
            Assert.Equal("Test", users[0].Name);
        }
    }
}