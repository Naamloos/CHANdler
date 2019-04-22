function initializepage(){
    var urlparams = new URLSearchParams(location.search);

    // test data
    var board_tag = urlparams.get("board");
    if(board_tag != 'c'){
        location.href = "boardlist.html"
        return;
    }
    var board_title = "CHANdler main test board";
    var board_welcome = "This is the main test board for CHANdler.";
    var board_img = "https://i.kym-cdn.com/photos/images/newsfeed/000/779/388/d33.jpg";

    // insert board info
    var elements = document.getElementsByClassName("board_info");
    for(var i = 0; i < elements.length; i++){
        var element = elements[i]
        element.innerHTML = "/" + board_tag + "/: " + board_title;
    }

    // insert board welcome
    var elements = document.getElementsByClassName("board_welcome");
    for(var i = 0; i < elements.length; i++){
        var element = elements[i]
        element.innerHTML = board_welcome;
    }

    // insert board name
    var elements = document.getElementsByClassName("board_title");
    for(var i = 0; i < elements.length; i++){
        var element = elements[i]
        element.innerHTML = board_title;
    }

    // insert board tag
    var elements = document.getElementsByClassName("board_tag");
    for(var i = 0; i < elements.length; i++){
        var element = elements[i]
        element.innerHTML = board_tag;
    }

    // insert board img
    var elements = document.getElementsByClassName("board_img");
    for(var i = 0; i < elements.length; i++){
        var element = elements[i]
        element.src = board_img;
    }

    var threadbox = document.getElementById("boardthreads");
    var threads = getThreads(board_tag);
    for(var i = 0; i < threads.length; i++)
    {
        var header = document.createElement("div");
        header.className = "threadheader";
        header.innerHTML = "<b>Thread by: " + threads[i].poster + "</b>";

        var text = document.createElement("div");
        text.className = "threadtext";
        text.innerHTML = threads[i].text;

        var thread = document.createElement("div");
        thread.className = "thread";

        var comments = getPosts(i);
        for(var j = 0; j <  comments.length; j++){
            var comment = document.createElement("div");
            var commenter = document.createElement("p");
            commenter.innerHTML = "<b>"+"Anonymous"+"</b>";
            var commenttext = document.createElement("p");
            commenttext.innerHTML = comments[j];
            commenttext.className = "commenttext";

            comment.className = "comment";
            comment.appendChild(commenter);
            comment.appendChild(commenttext);
            text.appendChild(comment);
        }
        thread.appendChild(header);
        thread.appendChild(text);
        // add comments here

        threadbox.appendChild(thread);
    }
}