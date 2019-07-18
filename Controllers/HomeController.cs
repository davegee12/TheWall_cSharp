using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheWall_cSharp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace TheWall_cSharp.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        // Display Index Page
        [HttpGet("")]
        public ViewResult Index()
        {
            return View("Index");
        }

        // Create RegUser POST route
        [HttpPost("create/register")]
        public IActionResult CreateRegUser(RegUser newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<RegUser> Hasher = new PasswordHasher<RegUser>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    dbContext.Add(newUser);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("LoggedInId", newUser.RegUserId);
                    return RedirectToAction("Wall");
                }
            }
            else
            {
                return View("Index");
            }
        }

        // Login LogUser POST route
        [HttpPost("login")]
        public IActionResult CreateLogUser(LogUser LoggedIn)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == LoggedIn.LogEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LogUser>();
                var result = hasher.VerifyHashedPassword(LoggedIn, userInDb.Password, LoggedIn.LogPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("LoggedInId", userInDb.RegUserId);
                return RedirectToAction("Wall");
            }
            else
            {
                return View("Index");
            }
        }

        // Log Out
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        // Dashboard Display Page
        [HttpGet("wall")]
        public IActionResult Wall()
        {
            int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
            if (IntVariable == null)
            {
                HttpContext.Session.Clear();
                ModelState.AddModelError("LoggedIn", "Please log in");
                return RedirectToAction("Index");
            }
            else
            {
                var LoggedInUser = dbContext.Users
                .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));

                List<Message> AllMessages = dbContext.Messages
                .Include(d => d.MessageCreator)
                .Include(a => a.CommentsOnMessage)
                .ThenInclude(v => v.UsersWhoLikedThisComment)
                .Include(e => e.UsersWhoLikedThisMessage)
                .OrderByDescending(i => i.CreatedAt)
                .ToList();

                ViewBag.LoggedInUser = LoggedInUser;
                ViewBag.AllMessages = AllMessages;
                return View("Wall");
            }
        }

        // Create Message POST route
        [HttpPost("create/message")]
        public IActionResult CreateMessage(Message newMessage)
        {
            if(ModelState.IsValid)
            {
                // grab the logged in user
                int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
                RegUser LoggedInUser = dbContext.Users
                .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));

                newMessage.RegUserId = Convert.ToInt32(IntVariable);
                newMessage.MessageCreator = LoggedInUser;
                dbContext.Messages.Add(newMessage);
                dbContext.SaveChanges();

                return RedirectToAction("Wall");
            }
            else
            {
                // grab the logged in user
                int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
                var LoggedInUser = dbContext.Users
                .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));

                // grab all messages including comments
                List<Message> AllMessages = dbContext.Messages
                .Include(d => d.MessageCreator)
                .Include(a => a.CommentsOnMessage)
                .ThenInclude(v => v.UsersWhoLikedThisComment)
                .Include(e => e.UsersWhoLikedThisMessage)
                .OrderByDescending(i => i.CreatedAt)
                .ToList();

                ViewBag.AllMessages = AllMessages;
                ViewBag.LoggedInUser = LoggedInUser;
                return View("Wall");
            }
        }

        // Create Comment POST route
        [HttpPost("create/comment")]
        public IActionResult CreateComment(Comment content)
        {
            if(ModelState.IsValid)
            {
                // grab the logged in user
                int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
                RegUser LoggedInUser = dbContext.Users
                .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));

                // grab the message to comment on

                content.RegUserId = Convert.ToInt32(IntVariable);
                dbContext.Comments.Add(content);
                dbContext.SaveChanges();

                return RedirectToAction("Wall");
            }
            else
            {
                // grab the logged in user
                int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
                var LoggedInUser = dbContext.Users
                .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));

                // grab all messages including comments
                List<Message> AllMessages = dbContext.Messages
                .Include(d => d.MessageCreator)
                .Include(a => a.CommentsOnMessage)
                .ThenInclude(v => v.UsersWhoLikedThisComment)
                .Include(e => e.UsersWhoLikedThisMessage)
                .OrderByDescending(i => i.CreatedAt)
                .ToList();

                ViewBag.AllMessages = AllMessages;
                ViewBag.LoggedInUser = LoggedInUser;
                return View("Wall");
            }
        }

        // Delete Message
        [HttpGet("message/{id}/delete")]
        public IActionResult DeleteMessage(int id)
        {
            Message RetrievedMessage = dbContext.Messages.FirstOrDefault(m => m.MessageId == id);
            dbContext.Messages.Remove(RetrievedMessage);
            dbContext.SaveChanges();

            // grab the logged in user
            int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
            var LoggedInUser = dbContext.Users
            .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));

            // grab all messages including comments
            List<Message> AllMessages = dbContext.Messages
            .Include(d => d.MessageCreator)
            .Include(a => a.CommentsOnMessage)
            .ThenInclude(v => v.UsersWhoLikedThisComment)
            .Include(e => e.UsersWhoLikedThisMessage)
            .OrderByDescending(i => i.CreatedAt)
            .ToList();

            ViewBag.AllMessages = AllMessages;
            ViewBag.LoggedInUser = LoggedInUser;
            return RedirectToAction("Wall");
        }

        // Delete Comment
        [HttpGet("comment/{id}/delete")]
        public IActionResult DeleteComment(int id)
        {
            Comment RetrievedComment = dbContext.Comments.FirstOrDefault(c => c.CommentId == id);
            dbContext.Comments.Remove(RetrievedComment);
            dbContext.SaveChanges();

            // grab the logged in user
            int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
            var LoggedInUser = dbContext.Users
            .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));

            // grab all messages including comments
            List<Message> AllMessages = dbContext.Messages
            .Include(d => d.MessageCreator)
            .Include(a => a.CommentsOnMessage)
            .ThenInclude(v => v.UsersWhoLikedThisComment)
            .Include(e => e.UsersWhoLikedThisMessage)
            .OrderByDescending(i => i.CreatedAt)
            .ToList();

            ViewBag.AllMessages = AllMessages;
            ViewBag.LoggedInUser = LoggedInUser;
            return RedirectToAction("Wall");
        }

        // Message Like Button
        [HttpGet("{id}/add/messagelike")]
        public IActionResult AddLikeToMessage(int id)
        {
            int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
            RegUser LoggedInUser = dbContext.Users
            .Include(user => user.MessagesLiked)
            .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));

            Message mess = dbContext.Messages
            .FirstOrDefault(i => i.MessageId == id);

            MessageLike like = new MessageLike();
            like.RegUserId = LoggedInUser.RegUserId;
            like.MessageId = id;
            dbContext.MessageLikes.Add(like);
            dbContext.SaveChanges();

            return RedirectToAction("Wall");
        }

        // Message Unlike Button
        [HttpGet("{id}/remove/messagelike")]
        public IActionResult RemoveLikeFromMessage(int id)
        {
            int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
            RegUser LoggedInUser = dbContext.Users
            .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));

            MessageLike deadlike = dbContext.MessageLikes
            .FirstOrDefault(i => i.MessageId == id && i.RegUserId == LoggedInUser.RegUserId);

            dbContext.MessageLikes.Remove(deadlike);
            dbContext.SaveChanges();

            return RedirectToAction("Wall");
        }

        // Comment Like Button
        [HttpGet("{id}/add/commentlike")]
        public IActionResult AddLikeToComment(int id)
        {
            int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
            RegUser LoggedInUser = dbContext.Users
            .Include(user => user.CommentsLiked)
            .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));

            Comment comm = dbContext.Comments
            .FirstOrDefault(i => i.CommentId == id);

            CommentLike like = new CommentLike();
            like.RegUserId = LoggedInUser.RegUserId;
            like.CommentId = id;
            dbContext.CommentLikes.Add(like);
            dbContext.SaveChanges();

            return RedirectToAction("Wall");
        }

        // Comment Unlike Button
        [HttpGet("{id}/remove/commentlike")]
        public IActionResult RemoveLikeFromComment(int id)
        {
            int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
            RegUser LoggedInUser = dbContext.Users
            .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));

            CommentLike deadlike = dbContext.CommentLikes
            .FirstOrDefault(i => i.CommentId == id && i.RegUserId == LoggedInUser.RegUserId);

            dbContext.CommentLikes.Remove(deadlike);
            dbContext.SaveChanges();

            return RedirectToAction("Wall");
        }
    }
}
