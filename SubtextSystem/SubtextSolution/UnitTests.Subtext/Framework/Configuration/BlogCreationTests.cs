using System;
using NUnit.Framework;
using Subtext.Framework;
using Subtext.Framework.Configuration;
using Subtext.Framework.Exceptions;
using Subtext.Framework.Providers;

namespace UnitTests.Subtext.Framework.Configuration
{
	/// <summary>
	/// These are unit tests specifically of the blog creation process, 
	/// as there are many validation rules involved.
	/// </summary>
	[TestFixture]
	public class BlogCreationTests
	{
		/// <summary>
		/// Ensures that creating a blog will hash the password 
		/// if UseHashedPassword is set in web.config (as it should be).
		/// </summary>
		[Test]
		public void CreatingBlogHashesPassword()
		{
			string password = "MyPassword";
			string hashedPassword = Security.HashPassword(password);
            
			Config.AddBlogConfiguration("", "username", password, "LocaLhost", "MyBlog1");
			BlogConfig config = Config.GetConfig("localhost", "MyBlog1");

			Config.Settings.UseHashedPasswords = true;
			Assert.IsTrue(Config.Settings.UseHashedPasswords, "This test is voided because we're not hashing passwords");
			Assert.AreEqual(hashedPassword, config.Password, "The password wasn't hashed.");
		}

		/// <summary>
		/// Ensures that creating a blog will hash the password 
		/// if UseHashedPassword is set in web.config (as it should be).
		/// </summary>
		[Test]
		public void ModifyingBlogHashesPassword()
		{
			string password = "My Password";
			string hashedPassword = Security.HashPassword(password);
            
			Config.AddBlogConfiguration("", "username", "something", "LocaLhost", "MyBlog1");
			BlogConfig config = Config.GetConfig("localhost", "MyBlog1");
			Config.Settings.UseHashedPasswords = true;
			
			config.Password = password;
			Assert.AreEqual(password, config.Password, "Passwords aren't hashed till they're saved. Otherwise loading a config would hash the hash.");
		
			Config.UpdateConfigData(config);

			Assert.AreEqual(hashedPassword, config.Password, "The password wasn't hashed.");
		}

		/// <summary>
		/// If a blog already exists with a domain name and application, one 
		/// cannot create a blog with the same domain name and no application.
		/// </summary>
		[Test, ExpectedException(typeof(BlogRequiresApplicationException))]
		public void CreatingBlogWithDuplicateHostNameRequiresApplicationName()
		{
			Config.AddBlogConfiguration("", "username", "password", "LocaLhost", "MyBlog1");
			Config.AddBlogConfiguration("", "username", "password", "LocaLhost", string.Empty);
		}

		/// <summary>
		/// Make sure adding two distinct blogs doesn't raise an exception.
		/// </summary>
		[Test]
		public void AddingDistinctBlogsIsFine()
		{
			Config.AddBlogConfiguration("title", "username", "password", "www.example.com", string.Empty);
			Config.AddBlogConfiguration("title", "username", "password", "www2.example.com", string.Empty);
			Config.AddBlogConfiguration("title", "username", "password", "example.org", string.Empty);
			Config.AddBlogConfiguration("title", "username", "password", "localhost", "Blog1");
			Config.AddBlogConfiguration("title", "username", "password", "localhost", "Blog2");
			Config.AddBlogConfiguration("title", "username", "password", "localhost", "Blog3");
		}

		/// <summary>
		/// Ensures that one cannot create a blog with a duplicate host 
		/// as another blog when both have no application specified.
		/// </summary>
		[Test, ExpectedException(typeof(BlogDuplicationException))]
		public void CreateBlogCannotCreateOneWithDuplicateHostAndNoApplication()
		{
			Config.AddBlogConfiguration("title", "username", "password", "LocaLhost", string.Empty);
			Config.AddBlogConfiguration("title", "username2", "password2", "localhost", string.Empty);
		}

		/// <summary>
		/// Ensures that one cannot create a blog with a duplicate application and host 
		/// as another blog.
		/// </summary>
		[Test, ExpectedException(typeof(BlogDuplicationException))]
		public void CreateBlogCannotCreateOneWithDuplicateHostAndApplication()
		{
			Config.AddBlogConfiguration("title", "username", "password", "localhost", "MyBlog");
			Config.AddBlogConfiguration("title", "username2", "password2", "Localhost", "MyBlog");
		}

		/// <summary>
		/// Ensures that one cannot update a blog to have a duplicate application and host 
		/// as another blog.
		/// </summary>
		[Test, ExpectedException(typeof(BlogDuplicationException))]
		public void UpdateBlogCannotConflictWithDuplicateHostAndApplication()
		{
			Config.AddBlogConfiguration("title", "username", "password", "localhost", "MyBlog");
			Config.AddBlogConfiguration("title", "username2", "password2", "example.com", "MyBlog");
			BlogConfig config = Config.GetConfig("example.com", "MyBlog");
			config.Host = "localhost";
			
			Config.UpdateConfigData(config);
		}

		/// <summary>
		/// Ensures that one update a blog to have a duplicate host 
		/// as another blog when both have no application specified.
		/// </summary>
		[Test, ExpectedException(typeof(BlogDuplicationException))]
		public void UpdateBlogCannotConflictWithDuplicateHost()
		{
			Config.AddBlogConfiguration("title", "username", "password", "localhost", string.Empty);
			Config.AddBlogConfiguration("title", "username2", "password2", "example.com", string.Empty);
			BlogConfig config = Config.GetConfig("example.com", string.Empty);
			config.Host = "localhost";
			
			Config.UpdateConfigData(config);
		}

		/// <summary>
		/// Ensures that creating a blog cannot "hide" another blog. Read the 
		/// remarks for more details.
		/// </summary>
		/// <remarks>
		/// <p>This exception occurs when adding a blog with the same hostname as another blog, 
		/// but the original blog does not have an application name defined.</p>  
		/// <p>For example, if there exists a blog with the host "www.example.com" with no 
		/// application defined, and the admin adds another blog with the host "www.example.com" 
		/// and application as "MyBlog", this creates a multiple blog situation in the example.com 
		/// domain.  In that situation, each example.com blog MUST have an application name defined. 
		/// The URL to the example.com with no application becomes the aggregate blog.
		/// </p>
		/// </remarks>
		[Test, ExpectedException(typeof(BlogHiddenException))]
		public void CreateBlogCannotHideAnotherBlog()
		{
			Config.AddBlogConfiguration("title", "username", "password", "www.example.com", string.Empty);
			Config.AddBlogConfiguration("title", "username", "password", "Example.com", "MyBlog");
		}

		/// <summary>
		/// Ensures that updating a blog cannot "hide" another blog. Read the 
		/// remarks for more details.
		/// </summary>
		/// <remarks>
		/// <p>This exception occurs when adding a blog with the same hostname as another blog, 
		/// but the original blog does not have an application name defined.</p>  
		/// <p>For example, if there exists a blog with the host "www.example.com" with no 
		/// application defined, and the admin adds another blog with the host "www.example.com" 
		/// and application as "MyBlog", this creates a multiple blog situation in the example.com 
		/// domain.  In that situation, each example.com blog MUST have an application name defined. 
		/// The URL to the example.com with no application becomes the aggregate blog.
		/// </p>
		/// </remarks>
		[Test]
		public void UpdatingBlogCannotHideAnotherBlog()
		{
			Config.AddBlogConfiguration("title", "username", "password", "www.mydomain.com", string.Empty);
			
			BlogConfig config = Config.GetConfig("www.mydomain.com", string.Empty);
			config.Host = "mydomain.com";
			config.Application = "MyBlog";
			Config.UpdateConfigData(config);
		}

		/// <summary>
		/// If a blog already exists with a domain name and application, one 
		/// cannot modify another blog to have the same domain name, but with no application.
		/// </summary>
		[Test, ExpectedException(typeof(BlogRequiresApplicationException))]
		public void UpdatingBlogWithDuplicateHostNameRequiresApplicationName()
		{
			Config.AddBlogConfiguration("title", "username", "password", "LocaLhost", "MyBlog1");
			Config.AddBlogConfiguration("title", "username", "password", "example.com", string.Empty);

			BlogConfig config = Config.GetConfig("www.example.com", string.Empty);
			config.Host = "localhost";
			config.Application = string.Empty;
			Config.UpdateConfigData(config);
		}

		/// <summary>
		/// This really tests that looking for duplicates doesn't 
		/// include the blog being edited.
		/// </summary>
		[Test]
		public void UpdatingBlogIsFine()
		{
			Config.AddBlogConfiguration("title", "username", "password", "www.example.com", string.Empty);
			BlogConfig config = Config.GetConfig("www.EXAMPLE.com", string.Empty);
			config.BlogID = 1;			
			Assert.IsTrue(Config.UpdateConfigData(config), "Updating blog config should return true.");
		}

		/// <summary>
		/// Makes sure that every invalid character is checked 
		/// within the application name.
		/// </summary>
		[Test]
		public void EnsureInvalidCharactersMayNotBeUsedInApplicationName()
		{
			string[] badNames = {".name", "a{b", "a}b", "a[e", "a]e", "a/e",@"a\e", "a@e", "a!e", "a#e", "a$e", "a'e", "a%", ":e", "a^", "ae&", "*ae", "a(e", "a)e", "a?e", "+a", "e|", "a\"", "e=", "a'", "e<", "a>e", "a;", ",e", "a e"};
			foreach(string badName in badNames)
			{
				Assert.IsFalse(Config.IsValidApplicationName(badName), badName + " is not a valid app name.");
			}
		}

		#region Invalid Application Name Tests... There's a bunch...
		/// <summary>
		/// Tests that creating a blog with a reserved keyword (bin) is not allowed.
		/// </summary>
		[Test, ExpectedException(typeof(InvalidApplicationNameException))]
		public void CannotCreateBlogWithApplicationNameBin()
		{
			Config.AddBlogConfiguration("title", "blah", "blah", "localhost", "bin");
		}

		/// <summary>
		/// Tests that modifying a blog with a reserved keyword (bin) is not allowed.
		/// </summary>
		[Test, ExpectedException(typeof(InvalidApplicationNameException))]
		public void CannotRenameBlogToHaveApplicationNameBin()
		{
			Config.AddBlogConfiguration("title", "blah", "blah", "localhost", "Anything");
			BlogConfig config = Config.GetConfig("localhost", "Anything");
			config.Application = "bin";

			Config.UpdateConfigData(config);
		}

		/// <summary>
		/// Tests that creating a blog with a reserved keyword (archive) is not allowed.
		/// </summary>
		[Test, ExpectedException(typeof(InvalidApplicationNameException))]
		public void CannotCreateBlogWithApplicationNameArchive()
		{
			Config.AddBlogConfiguration("title", "blah", "blah", "localhost", "archive");
			BlogConfig config = Config.GetConfig("localhost", "archive");
			config.Application = "archive";

			Config.UpdateConfigData(config);
		}

		/// <summary>
		/// Tests that creating a blog that ends with . is not allowed
		/// </summary>
		[Test, ExpectedException(typeof(InvalidApplicationNameException))]
		public void CannotCreateBlogWithApplicationNameEndingWithDot()
		{
			Config.AddBlogConfiguration("title", "blah", "blah", "localhost", "archive.");
		}

		/// <summary>
		/// Tests that creating a blog that starts with . is not allowed
		/// </summary>
		[Test, ExpectedException(typeof(InvalidApplicationNameException))]
		public void CannotCreateBlogWithApplicationNameStartingWithDot()
		{
			Config.AddBlogConfiguration("title", "blah", "blah", "localhost", ".archive");
		}

		/// <summary>
		/// Tests that creating a blog that contains invalid characters is not allowed.
		/// </summary>
		[Test, ExpectedException(typeof(InvalidApplicationNameException))]
		public void CannotCreateBlogWithApplicationNameWithInvalidCharacters()
		{
			Config.AddBlogConfiguration("title", "blah", "blah", "localhost", "My!Blog");
		}
		#endregion

		[SetUp]
		public void SetUp()
		{
			//This file needs to be there already.
			UnitTestHelper.UnpackEmbeddedResource("App.config", "UnitTests.Subtext.dll.config");
			
			//Confirm app settings
			Assert.AreEqual("~/Admin/Resources/PageTemplate.ascx", System.Configuration.ConfigurationSettings.AppSettings["Admin.DefaultTemplate"]) ;

			UnitTestDTOProvider dtoProvider = (UnitTestDTOProvider)DTOProvider.Instance();
			dtoProvider.ClearBlogs();
		}

		[TearDown]
		public void TearDown()
		{
		}
	}
}
