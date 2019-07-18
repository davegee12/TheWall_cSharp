using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace TheWall_cSharp.Models
{
    public class Message
    {
        [Key]
        public int MessageId{get;set;}

        [Required]
        public string Text {get;set;}

        // Foreign Key to User
        public int RegUserId{get;set;}
        // navigation
        public RegUser MessageCreator{get;set;}

        // Foreign Key to Comment
        public List<Comment> CommentsOnMessage{get;set;}

        // Many to Many Navigations
        public List<MessageLike> UsersWhoLikedThisMessage{get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}