
//Shared methods
var Shared = {
    //Scroll to an element on the page
    ScrollTo: function (element) {
        $("html").animate({
            scrollTop: $(element).offset().top - $(element).outerHeight()
        }, 650);
    }
}

//Post Detail methods
var PostDetail = {
    //Initialize page
    Init: function (request, loadEditor) {
        if (loadEditor) {
            $.validator.unobtrusive.parse("form");
            Shared.ScrollTo("#content-editor");
        }
        $(".post-comment .fa-comment-alt").on("click", function () {
            PostDetail.LoadContentEditor(request);
        });
        $("#content-editor").on("click", "#cancel-comment", function () {
            $("#content-editor").html("");
        });
    },
    //Load replies drilldown for comments
    LoadDrillDown: function (request) {
        $.ajax({
            url: request.url,
            type: "GET",
            data: { postId: request.postId },
            success: function (html) {
                $(".comment-drilldown-" + request.postId).html(html);
            }
        });
    },
    //Load the content editor form
    LoadContentEditor: function (request) {
        $.ajax({
            url: request.url,
            method: "GET",
            data: { postId: request.postId },
            success: function (html) {
                $("#content-editor").html(html);
                $.validator.unobtrusive.parse("form");
                Shared.ScrollTo("#content-editor");
            }
        });
    }
};

//Post methods
var Post = {
    //Ajax request to delete a post
    DeletePost: function (request) {
        $.ajax({
            url: request.url,
            type: "DELETE",
            data: { id: request.postId },
            success: function (url) {
                window.location.href = url;
            }
        });
    },
    //Show delete modal and format it's click events
    DeletePostModal: function (request) {
        $("#confirm-delete-btn").off("click").on("click", function () {
            Post.DeletePost(request);
        });
        $("#delete-confirm-modal").modal("show");
    },
    //Ajax request to like a post
    LikePost: function (request) {
        $.ajax({
            url: request.url,
            type: "POST",
            data: { id: request.postId },
            success: function (data) {
                Post.SetLikes(data, request.postId);
            }
        });
    },
    //Format and update the likes metric with a new value
    SetLikes: function (count, postId) {
        let value = $("#post-" + postId + " .like-count");
        let label = $("#post-" + postId + " .like-count-label");
        value.text(count);
        if (count == 0) {
            value.css("display", "none");
            label.css("display", "none");
            if (value.parent().children().length < 4) {
                value.parent().css("display", "none");
            }
        }
        else {
            if (count == 1)
                label.text("Like");
            else
                label.text("Likes");

            value.parent().css("display", "block");
            value.css("display", "inline-block");
            label.css("display", "inline-block");
        }
        $("#post-" + postId + " .post-like").toggleClass("active");
    }
};

//Comment methods
var Comment = {
    //Ajax request to edit a comment
    EditComment: function (request) {
        $.ajax({
            url: request.url,
            type: "GET",
            data: { postId: request.postId },
            success: function (html) {
                $("#content-editor").html(html);
                $.validator.unobtrusive.parse("form");
                Shared.ScrollTo("#content-editor");
            }
        });
    },
    //Ajax request to delete a comment
    DeleteComment: function (request) {
        $.ajax({
            url: request.url,
            type: "DELETE",
            data: { postId: request.postId },
            success: function (url) {
                window.location.href = url;
            }
        });
    },
    //Show delete modal and update click events
    DeleteCommentModal: function (request) {
        $("#confirm-delete-btn").off("click").on("click", function () {
            Comment.DeleteComment(request);
        });
        $("#delete-confirm-modal").modal("show");
    },
    //Ajax request to like a comment
    LikeComment: function (request) {
        $.ajax({
            url: request.url,
            type: "POST",
            data: { id: request.postId },
            success: function (data) {
                Comment.SetLikes(data, request.postId);
            }
        });
    },
    //Format and update like metrics for comment
    SetLikes: function (count, postId) {
        let value = $("#comment-" + postId + " .like-count").first();
        value.text(count);

        $("#comment-" + postId + " .comment-like").first().toggleClass("active");
    }
};

//Topic methods
var Topic = {
    //Ajax request to delete a comment
    DeleteTopic: function (request) {
        $.ajax({
            url: request.url,
            method: "DELETE",
            data: { id: request.topicId },
            success: function (url) {
                window.location.href = url;
            }
        });
    },
    //Show delete modal and update click events
    DeleteTopicModal: function (request) {
        $("#confirm-delete-btn").off("click").on("click", function () {
            Topic.DeleteTopic(request);
        });
        $("#delete-confirm-modal").modal("show");
    }
};

var Topics = {
    Initialize: function () {
        $("#topics-table").DataTable();
    }
};

var Users = {
    Initialize: function () {
        $("#user-table").DataTable();
    },
    SuspendUserModal: function (request) {
        $("#confirm-suspend-btn").off("click").on("click", function () {
            Users.SuspendUser(request);
        });
        $("#suspend-confirm-modal").modal("show");
    },
    DeleteUserModal: function (request) {
        $("#confirm-delete-btn").off("click").on("click", function () {
            Users.DeleteUser(request);
        });
        $("#delete-confirm-modal").modal("show");
    },
    SuspendUser: function (request) {
        $.ajax({
            url: request.url,
            method: "POST",
            data: { id: request.userAccountId },
            success: function (url) {
                window.location.href = url;
            }
        });
    },
    DeleteUser: function (request) {
        $.ajax({
            url: request.url,
            method: "DELETE",
            data: { id: request.userAccountId },
            success: function (url) {
                window.location.href = url;
            }
        });
    }
};

var Reports = {
    Initialize: function () {
        $("#report-table").DataTable();
    }
}

var ReportDetail = {
    ResolveReport: function (request) {
        $.ajax({
            url: request.url,
            method: "DELETE",
            data: { id: request.reportId },
            success: function (url) {
                window.location.href = url;
            }
        });
    }
}
