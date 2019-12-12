using GucciBazaar.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GucciBazaar.Startup))]
namespace GucciBazaar
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateAdminUserAndApplicationRoles();        }

        private void CreateAdminUserAndApplicationRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // Se adauga rolurile aplicatiei
            if (!roleManager.RoleExists("Administrator"))
            {
                // Se adauga rolul de administrator
                var role = new IdentityRole
                {
                    Name = "Administrator"
                };
                roleManager.Create(role);
                // se adauga utilizatorul administrator
                var admin = new ApplicationUser
                {
                    UserName = "admin@test.com",
                    Email = "admin@test.com"
                };
                var adminCreated = UserManager.Create(admin, "Test123_");
                if (adminCreated.Succeeded)
                {
                    UserManager.AddToRole(admin.Id, "Administrator");
                }
            }

            if (!roleManager.RoleExists("Editor"))
            {
                var role = new IdentityRole
                {
                    Name = "Editor"
                };
                roleManager.Create(role);
                var editor = new ApplicationUser
                {
                    UserName = "editor@test.com",
                    Email = "editor@test.com"
                };
                var editorCreated = UserManager.Create(editor, "Test123_");
                if (editorCreated.Succeeded)
                {
                    UserManager.AddToRole(editor.Id, "Editor");
                }
            }
            if (!roleManager.RoleExists("User"))
            {
                var role = new IdentityRole
                {
                    Name = "User"
                };
                roleManager.Create(role);
                var user = new ApplicationUser
                {
                    UserName = "user@test.com",
                    Email = "user@test.com"
                };
                var userCreated = UserManager.Create(user, "Test123_");
                if (userCreated.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Editor");
                }
                var usercart = new ApplicationUser
                {
                    UserName = "usercart@test.com",
                    Email = "usercart@test.com"
                };
                var usercartCreated = UserManager.Create(usercart, "Test123_");
                if (usercartCreated.Succeeded)
                {
                    UserManager.AddToRole(usercart.Id, "Editor");
                }
            }
        }
    }
}
