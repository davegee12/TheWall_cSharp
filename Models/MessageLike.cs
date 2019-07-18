using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace TheWall_cSharp.Models
{
    public class MessageLike
    {
        [Key]
        public int MessageLikeId{get;set;}

        public int RegUserId{get;set;}
        public int MessageId{get;set;}

        public DateTime CreatedAt{get;set;} = DateTime.Now;
        public DateTime UpdateAt{get;set;} = DateTime.Now;

        public RegUser MessageLiker{get;set;}
        public Message MessageLiked{get;set;}
    }
}