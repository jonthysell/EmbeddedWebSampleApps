var checkStart = setInterval(function() {
    if (document.querySelector('#start-button') && document.getElementById('start-button')?.textContent === 'Run Benchmark') {
        console.log('MotionMark Start');
        document.getElementById('start-button')?.click();
        clearInterval(checkStart);
    }
}, 1000); // check every 1000ms

var checkResult = setInterval(function() {
    if (document.querySelector('.score') && document.getElementById('results')?.getElementsByClassName("score")[0]?.textContent) {
        const score = document.getElementById('results')?.getElementsByClassName("score")[0]?.textContent;
        if (score)
        {
            console.log('MotionMark End');
            console.log(`MotionMark Score: ${ score }`);
            clearInterval(checkResult);
            WebTesterNativeAPI.exitApp();
        }
    }
}, 1000); // check every 1000ms
