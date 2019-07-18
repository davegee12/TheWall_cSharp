using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace TheWall_cSharp.Models
{
    public class CommentLike
    {
        [Key]
        public int CommentLikeId{get;set;}

        public int RegUserId{get;set;}
        public int CommentId{get;set;}

        public DateTime CreatedAt{get;set;} = DateTime.Now;
        public DateTime UpdateAt{get;set;} = DateTime.Now;

        public RegUser CommentLiker{get;set;}
        public Comment CommentLiked{get;set;}
    }
}