async function initializeboard(){
    var urlparams = new URLSearchParams(location.search);

    // test data
    var board_tag = urlparams.get("board");
    var boardinfo = await getBoardDataAsync(board_tag);
    if(!boardinfo){
        location.href = "index.html"
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
        element.innerHTML = "/" + board_tag + "/ - " + board_title;
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
        element.innerHTML = "/" + board_tag + "/ - " + board_title;
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
        element.style = 'max-height: 180px;';
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
            img = '<img class="imgsmall" onclick="imgclick(this)" src="' + threads[i].image + '">';
        }

        var thrtext = threads[i].text;
        var linkregex = /https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/g;
        var ytregex = /^((?:https?:)?\/\/)?((?:www|m)\.)?((?:youtube\.com|youtu.be))(\/(?:[\w\-]+\?v=|embed\/|v\/)?)([\w\-]+)(\S+)?$/;

        var matches = thrtext.match(linkregex);
        console.log(matches);
        if(matches != null){
            for(var p = 0; p < matches.length; p++){
                if(matches[p] != null){
                    var isyt = matches[p].match(ytregex);
                    if(isyt){
                        thrtext = thrtext.replace(matches[p], '<iframe style="max-width: 80%; width: 320px;" src="https://www.youtube.com/embed/'+isyt[5]+'" frameborder="0" allowfullscreen></iframe>');
                    }else{
                        thrtext = thrtext.replace(matches[p], '<a href="' + matches[p] + '" target="_blank">'+matches[p]+'</a>');
                    }
                }
            }
        }
        thrtext = thrtext.replace(/(?:\r\n|\r|\n|&#xA;)/g, '<br>');


        text.innerHTML = img + '<p>' + thrtext + '</p><p><i><a href="new.html?board=' + board_tag + '&parent=' + threads[i].id + '">Reply</a></i>'
            +' <i><a href="delete.html?post='+threads[i].id+'&board='+board_tag+'">Delete</a></i></p>';

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
                    img = '<img class="imgsmall" onclick="imgclick(this)" src="' + comments[j].image + '">';
                }

                console.log(comments[j]);

                var cmtext = comments[j].text;
                var linkregex = /https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/g;
                var ytregex = /^((?:https?:)?\/\/)?((?:www|m)\.)?((?:youtube\.com|youtu.be))(\/(?:[\w\-]+\?v=|embed\/|v\/)?)([\w\-]+)(\S+)?$/;
                var matches = cmtext.match(linkregex);
                console.log(matches);
                if(matches != null){
                    for(var p = 0; p < matches.length; p++){
                        if(matches[p] != null){
                            var isyt = matches[p].match(ytregex);
                            if(isyt){
                                cmtext = cmtext.replace(matches[p], '<iframe style="max-width: 80%; width: 320px;" src="https://www.youtube.com/embed/'+isyt[5]+'" frameborder="0" allowfullscreen></iframe>');
                            }else{
                                cmtext = cmtext.replace(matches[p], '<a href="' + matches[p] + '" target="_blank">'+matches[p]+'</a>');
                            }
                        }
                    }
                }
                
                cmtext = cmtext.replace(/(?:\r\n|\r|\n|&#xA;)/g, '<br>');

                commenttext.innerHTML = img + "<p>" + cmtext + '</p><p><i><a href="delete.html?post='+comments[j].id+'">Delete</a></i></p>';;
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
    document.title = sitename;
    
    var bl = document.getElementById("boardlist");
    var boards = await getBoardsAsync();
    for(var i = 0; i < boards.length; i++){
        var board = document.createElement("p");
        var link = document.createElement("a");
        link.innerHTML = "/" + boards[i].tag + "/ - " + boards[i].name;
        link.href = "board.html?board=" + boards[i].tag;
        board.appendChild(link);
        bl.appendChild(board);
    }

    var name = document.getElementById("sitename");
    name.innerHTML = "Welcome to " + sitename;

    var logo = document.getElementById("sitelogo");
    logo.src = sitelogo;
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
    var pw = form.get("password");

    console.log(text);

    if(parentid == null){
        parentid = -1;
    }
    
    if(board == null){
        location.href = "index.html";
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
    if(pw == null){
        pw = "";
    }

    await makePost(text, username, parentid, board, image, "", pw)

    location.href = "board.html?board=" + board;
}

async function deletepost(){
    var form = new FormData(document.forms[0]);
    var urlparams = new URLSearchParams(location.search);
    var post = urlparams.get("post");
    
    var password = form.get("password");
    
    if(password == null || post == null){
        location.href = "index.html";
        return;
    }

    await deleteJson(server + "/api/thread/delete?postid=" + post, password);

    location.href = "index.html";
}

function imgclick(image){
    if(image.classList.contains("imgsmall")){
        image.classList.remove("imgsmall");
        image.classList.add("imglarge");
    }else{
        image.classList.add("imgsmall");
        image.classList.remove("imglarge");
    }
}

function backtoboard(){
    var urlparams = new URLSearchParams(location.search);
    var board = urlparams.get("board");

    location.href="board.html?board=" + board;
}

async function initializethread(){
    var urlparams = new URLSearchParams(location.search);

    var thread = await getThread(urlparams.get("id"));
    // test data
    var board_tag = thread.boardtag;

    var boardinfo = await getBoardDataAsync(board_tag);
    if(!boardinfo){
        location.href = "index.html"
        return;
    }
    var board_title = boardinfo.name;
    var board_welcome = boardinfo.description;
    var board_img = "";
    if(boardinfo.imageUrl != null){
        board_img = boardinfo.imageUrl;
    }

    // insert board name
    var elements = document.getElementsByClassName("board_title");
    for(var i = 0; i < elements.length; i++){
        var element = elements[i]
        element.innerHTML = "/" + board_tag + "/ - " + board_title;
    }

    // insert board tag
    var elements = document.getElementsByClassName("board_tag");
    for(var i = 0; i < elements.length; i++){
        var element = elements[i]
        element.innerHTML = board_tag;
    }

    var threadbox = document.getElementById("thread");

    /////////////////////
    var header = document.createElement("div");
    header.className = "threadheader";
    header.innerHTML = "<b>Thread by: " + thread.username + "</b> ID: " + thread.id;

    var text = document.createElement("div");
    text.className = "threadtext";
    var img = "";
    if(thread.image != null || thread.image != ""){
        img = '<img class="imgsmall" onclick="imgclick(this)" src="' + thread.image + '">';
    }

    var thrtext = thread.text;
    var linkregex = /https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/g;

    var matches = thrtext.match(linkregex);
    console.log(matches);
    if(matches != null){
        for(var p = 0; p < matches.length; p++){
            if(matches[p] != null){
                thrtext = thrtext.replace(matches[p], '<a href="' + matches[p] + '" target="_blank">'+matches[p]+'</a>');
            }
        }
    }
    thrtext = thrtext.replace(/(?:\r\n|\r|\n|&#xA;)/g, '<br>');

    text.innerHTML = img + '<p>' + thrtext + '</p><p><i><a href="new.html?board=' + board_tag + '&parent=' + thread.id + '">Reply</a></i>'
        +' <i><a href="delete.html?post='+thread.id+'&board='+board_tag+'">Delete</a></i></p>';

    var thread = document.createElement("div");
    thread.className = "thread";

    var comments = await getPosts(thread.id);
    if(comments.length > 0){
        for(var j = 0; j <  comments.length; j++){
            var comment = document.createElement("div");
            var commenter = document.createElement("p");
            commenter.innerHTML = "<b>"+comments[j].username+"</b> ID: " + comments[j].id;
            var commenttext = document.createElement("div");

            var img = "";
            if(comments[j].image != null || comments[j].image != ""){
                img = '<img class="imgsmall" onclick="imgclick(this)" src="' + comments[j].image + '">';
            }

            console.log(comments[j]);

            var cmtext = comments[j].text;
            var linkregex = /https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/g;
            var matches = cmtext.match(linkregex);
            console.log(matches);
            if(matches != null){
                for(var p = 0; p < matches.length; p++){
                    if(matches[p] != null){
                        cmtext = cmtext.replace(matches[p], '<a href="' + matches[p] + '" target="_blank">'+matches[p]+'</a>');
                    }
                }
            }
            
            cmtext = cmtext.replace(/(?:\r\n|\r|\n|&#xA;)/g, '<br>');

            commenttext.innerHTML = img + "<p>" + cmtext + '</p><p><i><a href="delete.html?post='+comments[j].id+'">Delete</a></i></p>';;
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

    ////////////

    var newthreadlink = document.getElementById("backlink");
    newthreadlink.innerHTML = '<a href="board.html?board='+board_tag+'">Back to board</a>';
}