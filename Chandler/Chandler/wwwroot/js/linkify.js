var posts1 = document.getElementsByClassName("_linkify");

for (let index = 0; index < posts1.length; index++) {
    const element = posts1[index];
    element.innerHTML = linkify(element.innerHTML);
}

function linkify(input) {
    var thrtext = input;
    var linkregex = /https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/g;
    var ytregex = /^((?:https?:)?\/\/)?((?:www|m)\.)?((?:youtube\.com|youtu.be))(\/(?:[\w\-]+\?v=|embed\/|v\/)?)([\w\-]+)(\S+)?$/;

    var matches = thrtext.match(linkregex);

    if (matches != null) {
        for (var p = 0; p < matches.length; p++) {
            if (matches[p] != null) {
                var isyt = matches[p].match(ytregex);
                if (isyt) {
                    thrtext = thrtext.replace(matches[p], '<iframe style="max-width: 80%; width: 320px;" src="https://www.youtube.com/embed/' + isyt[5] + '" frameborder="0" allowfullscreen></iframe>');
                } else {
                    thrtext = thrtext.replace(matches[p], '<a href="' + matches[p] + '" target="_blank">' + matches[p] + '</a>');
                }
            }
        }
    }
    thrtext = thrtext.replace(/(?:\r\n|\r|\n|&#xA;)/g, '<br>');

    return thrtext;
}