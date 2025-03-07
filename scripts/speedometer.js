var checkStart = setInterval(function() {
    if (document.querySelector('.start-tests-button')) {
        console.log('Speedometer Start');
        document.getElementsByClassName('start-tests-button')[0].click();
        clearInterval(checkStart);
    }
}, 1000); // check every 1000ms

var checkResult = setInterval(function() {
    if (document.querySelector('#result-number') && document.getElementById("result-number").textContent) {
        const result = document.getElementById("result-number").textContent;
        if (result)
        {
            console.log('Speedometer End');
            console.log(`Speedometer Result: ${ result }`);
            clearInterval(checkResult);
            WebTesterNativeAPI.exitApp();
        }
    }
}, 1000); // check every 1000ms