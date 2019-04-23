async function initializeboard(){
    var urlparams = new URLSearchParams(location.search);

    // test data
    var board_tag = urlparams.get("board");
    var boardinfo = await getBoardDataAsync(board_tag);
    if(!boardinfo){
        location.href = "boardlist.html"
        return;
    }
    var board_title = boardinfo.name;
    var board_welcome = boardinfo.description;
    var board_img = "";
    if(boardinfo.imageUrl != null){
        board_img = boardinfo.imageUrl;
    }

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

async function initializeboardlist(){
    var bl = document.getElementById("boardlist");
    var boards = await getBoardsAsync();
    for(var i = 0; i < boards.length; i++){
        var board = document.createElement("p");
        var link = document.createElement("a");
        link.innerHTML = "/" + boards[i].tag + "/: " + boards[i].name;
        link.href = "index.html?board=" + boards[i].tag;
        board.appendChild(link);
        bl.appendChild(board);
    }
}