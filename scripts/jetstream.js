var checkStart = setInterval(function() {
    if (document.querySelector('#status') && !document.querySelector('#status').classList.contains('loading')) {
        console.log('JetStream Start');
        document.getElementById('status')?.getElementsByClassName('button')[0]?.click();
        clearInterval(checkStart);
    }
}, 1000); // check every 1000ms

var checkResult = setInterval(function() {
    if (document.querySelector('.score') && document.getElementById('result-summary')?.getElementsByClassName("score")[0]?.textContent) {
        const score = document.getElementById('result-summary')?.getElementsByClassName("score")[0]?.textContent;
        if (score)
        {
            console.log('JetStream End');
            console.log(`JetStream Score: ${ score }`);
            clearInterval(checkResult);
            WebTesterNativeAPI.exitApp();
        }
    }
}, 1000); // check every 1000ms
