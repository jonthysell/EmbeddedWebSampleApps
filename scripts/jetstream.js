var checkStart = setInterval(function() {
    if (document.querySelector('.button')) {
        console.log('JetStream Start');
        document.getElementsByClassName('button')[0].click();
        clearInterval(checkStart);
    }
}, 1000); // check every 1000ms

var checkResult = setInterval(function() {
    if (document.querySelector('.score') && document.getElementById('result-summary')?.getElementsByClassName("score")[0].textContent) {
        const score = document.getElementById('result-summary')?.getElementsByClassName("score")[0].textContent;
        if (score)
        {
            console.log('JetStream End');
            console.log(`JetStream Result: ${ score }`);
            clearInterval(checkResult);
            WebTesterNativeAPI.exitApp();
        }
    }
}, 1000); // check every 1000ms