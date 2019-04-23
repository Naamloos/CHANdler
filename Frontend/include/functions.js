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

function getThreads(boardtag){
    var threads = [];

    var thread = new Object();
    thread.text = "Anon, your waifu sucks. What do you say?";
    thread.image = "https://cdn.shopify.com/s/files/1/1061/1924/products/Emoji_Icon_-_Smirk_face_large.png?v=1542436013";
    thread.poster = "Anonymous";
    thread.time = 0;

    threads.push(thread);

    var thread = new Object();
    thread.text = "Another test thread";
    thread.image = "https://cdn.shopify.com/s/files/1/1061/1924/products/Emoji_Icon_-_Smirk_face_large.png?v=1542436013";
    thread.poster = "Anonymous";
    thread.time = 0;

    threads.push(thread);

    return threads;
}

function getPosts(threadid){
    if (threadid == 0)
        var posts = ["That's not very nice of you to say", "fuck you", "lorum ipsum"];
    if (threadid == 1)
        var posts = ["big kek", "yoink"];

    return posts;
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