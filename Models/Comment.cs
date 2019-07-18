using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace TheWall_cSharp.Models
{
    public class Comment
    {
        [Key]
        public int CommentId{get;set;}

        [Required]
        public string Text {get;set;}

        // Foreign Key to User
        public int RegUserId{get;set;}
        // navigation
        public RegUser CommentCreator{get;set;}

        // Foreign Key to Message
        public int MessageId{get;set;}
        // navigation
        public Message MessageCommentedOn{get;set;}

        // Many to Many Navigations
        public List<CommentLike> UsersWhoLikedThisComment{get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}