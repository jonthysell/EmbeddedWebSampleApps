var checkStart = setInterval(function() {
    if (document.querySelector('.start-tests-button')) {
        console.log('Speedometer Start');
        document.getElementsByClassName('start-tests-button')[0]?.click();
        clearInterval(checkStart);
    }
}, 1000); // check every 1000ms

var checkResult = setInterval(async function() {
    if (document.querySelector('#result-number') && document.getElementById("result-number")?.textContent) {
        const score = document.getElementById("result-number")?.textContent;
        if (score)
        {
            console.log('Speedometer End');
            console.log(`Speedometer Score: ${ score }`);
            await new Promise(r => setTimeout(r, 500));
            clearInterval(checkResult);
            WebTesterNativeAPI.exitApp();
        }
    }
}, 1000); // check every 1000ms
