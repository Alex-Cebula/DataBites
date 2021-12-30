using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula
{
    public static class Constants
    {
        public static class ContentType
        {
            public static readonly string Topic = "TOPIC";
            public static readonly string Post = "POST";
            public static readonly string Comment = "CMT";
            public static readonly string Reply = "REPLY";
        }
        public static class Role
        {
            public static readonly string Admin = "ADMIN";
            public static readonly string User = "USER";
        }
        public static class EditorType
        {
            public static readonly string Create = "CREATE";
            public static readonly string Edit = "EDIT";
        }
        public static class Reflection
        {
            //Shared properties
            public static class ContentProperty
            {
                public static readonly string UserAccountId = "UserAccountId";
                public static readonly string DateCreated = "DateCreated";
                public static readonly string Title = "Title";
            }
            public static class TopicProperty
            {
                public static readonly string Description = "Description";
                
            }
            public static class PostProperty
            {
                public static readonly string Body = "Body";
            }
        }
    }
}
