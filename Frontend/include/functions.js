// mock data for now

async function getBoardsAsync(){
    var dat = await grabJson(server + "/api/board/");
    console.log(dat);
    return dat;
}

async function getBoardDataAsync(tag){
    var dat = await grabJson(server + "/api/board/data?tag=" + tag)
    return dat;
}

async function getThreads(boardtag){
    var threads = await grabJson(server + "/api/thread?tag=" + boardtag)

    return threads;
}

async function getThread(id){
    var thread = await grabJson(server + "/api/thread/single?id=" + id);

    return thread;
}

async function getPosts(threadid){
    var posts = await grabJson(server + "/api/thread/post?thread=" + threadid);

    return posts;
}

async function makePost(text, username, parentid, board, image, topic, password){
    var post = new Object();
    post.text = text;
    post.username = username;
    post.parentid = parentid;
    post.boardtag = board;
    post.image = image;
    post.topic = topic;
    post.generatepass = password;
    await postJson(server + "/api/thread/create/", post);
}

async function postJson(url, json){
    var req = await fetch(url, {
        method: "POST",
        mode: "cors",
        cache: "no-cache",
        headers: {
            "Content-Type": "application/json",
            // "Content-Type": "application/x-www-form-urlencoded",
        },
        body: JSON.stringify(json)
    });

    console.log(req);

    return req.status != 400;
}

async function deleteJson(url, json){
    var req = await fetch(url, {
        method: "DELETE",
        mode: "cors",
        cache: "no-cache",
        headers: {
            "Content-Type": "application/json",
            // "Content-Type": "application/x-www-form-urlencoded",
        },
        body: JSON.stringify(json)
    });

    console.log(await req.text());

    return req.status != 400;
}

async function grabJson(url){
    console.log(url);
    var res = await fetch(url);

    console.log(res);

    if(res.status != 404){
        var json = await res.json();
        return json;
    }

    return false;
}