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
    var threads = await getThreads(board_tag);
    for(var i = 0; i < threads.length; i++)
    {
        var header = document.createElement("div");
        header.className = "threadheader";
        header.innerHTML = "<b>Thread by: " + threads[i].username + "</b> ID: " + threads[i].id;

        var text = document.createElement("div");
        text.className = "threadtext";
        var img = "";
        if(threads[i].image != null || threads[i].image != ""){
            img = '<a href="' + threads[i].image + '" target="_blank"><img style="max-width: 300px;" src="' + threads[i].image + '"></a>';
        }
        text.innerHTML = img + '<p>' + threads[i].text + '</p><p><i><a href="new.html?board=' + board_tag + '&parent=' + threads[i].id + '">Reply</a></i></p>';

        var thread = document.createElement("div");
        thread.className = "thread";

        var comments = await getPosts(threads[i].id);
        if(comments.length > 0){
            for(var j = 0; j <  comments.length; j++){
                var comment = document.createElement("div");
                var commenter = document.createElement("p");
                commenter.innerHTML = "<b>"+comments[j].username+"</b> ID: " + comments[j].id;
                var commenttext = document.createElement("div");

                var img = "";
                if(comments[j].image != null || comments[j].image != ""){
                    img = '<a href="' + comments[j].image + '" target="_blank"><img style="max-width: 300px;" src="' + comments[j].image + '"></a>';
                }

                console.log(comments[j]);

                commenttext.innerHTML = img + "<p>" + comments[j].text + '</p>';
                commenttext.className = "commenttext";

                comment.className = "comment";
                comment.appendChild(commenter);
                comment.appendChild(commenttext);
                text.appendChild(comment);
            }
        }
        thread.appendChild(header);
        thread.appendChild(text);
        // add comments here

        threadbox.appendChild(thread);
    }

    var newthreadlink = document.getElementById("newthreadlink");
    newthreadlink.innerHTML = '<a href="new.html?board='+board_tag+'">Post new thread</a>';
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

async function initializeaddpost(){

}

async function addpost(){
    var form = new FormData(document.forms[0]);
    var urlparams = new URLSearchParams(location.search);
    var board = urlparams.get("board");
    var parentid = urlparams.get("parent");
    
    var username = form.get("username");
    var text = form.get("text");
    var image = form.get("imageurl");

    console.log(text);

    if(parentid == null){
        parentid = -1;
    }
    
    if(board == null){
        location.href = "boardlist.html";
        return;
    }
    if(text == null){
        alert("Invalid text!");
        return;
    }
    if(username == null){
        username = "Anonymous";
    }
    if(image == null){
        image = "";
    }

    await makePost(text, username, parentid, board, image, "")

    location.href = "index.html?board=" + board;
}