const onpform = document.getElementById("onpform");
const calculatexform = document.getElementById("xform");
const calculaterangeform = document.getElementById("rangeform");
const formulaform = document.getElementById("formulaform");

//var link = 'https://localhost:5001/api/';
 //link = 'https://localhost:5001/api/';
var link = 'http://192.168.99.100:8001/api/';

var allgoodx = false;
var allgoodrange = false;
if (formulaform != null) {

    const inputFormula = formulaform.querySelector("textarea[name=formula]");

    const resultinfix = document.getElementById('resultinfix');
    const resultonp = document.getElementById('resultonp');
    resultinfix.style.display = "none";
    resultonp.style.display = "none";
    formulaform.onsubmit = function (e) {
        resultinfix.style.display = "none";
        resultonp.style.display = "none";
        e.preventDefault();
        if (inputFormula.value == "") {
            resultinfix.style.background = "#ad1f1f";
            resultinfix.style.borderColor = "#451717";
            resultinfix.innerHTML = "Błąd: Równanie jest puste";
        }
        else if (inputFormula.value.includes("=")) {
            resultinfix.style.background = "#ad1f1f";
            resultinfix.style.borderColor = "#451717";
            resultinfix.innerHTML = "Błąd: Równanie zawiera =";
        }
        else if (inputFormula.value.includes(" ")) {
            resultinfix.style.background = "#ad1f1f";
            resultinfix.style.borderColor = "#451717";
            resultinfix.innerHTML = "Błąd: Równanie zawiera spację";
        }
        else {
            var formValue = inputFormula.value;
            formValue = formValue.split("+").join("=");
            var request = new XMLHttpRequest();
            request.open('GET', link + 'tokens?formula=' + formValue, true);
            request.onerror = function () {
                alert("Połączenie z API zostało zerwane");
            }
            request.onload = function () {

                var data = JSON.parse(this.response);
                var infix;
                var rpn;
                resultinfix.innerHTML = resultonp.innerHTML = "";
                if (request.status >= 200 && request.status < 400) {

                    if (data.status == "ok") {
                        resultinfix.style.background = "#249424";
                        resultinfix.style.borderColor = "#0f4e0f";
                        resultonp.style.background = "#249424";
                        resultonp.style.borderColor = "#0f4e0f";
                        resultonp.style.display = "block";
                        infix = data.result.infix;
                        rpn = data.result.rpn;
                        resultinfix.innerHTML += "Infix: ";
                        infix.forEach(i => {
                            resultinfix.innerHTML += i + " ";
                        });
                        resultonp.innerHTML += "Onp: ";
                        rpn.forEach(r => {
                            resultonp.innerHTML += r + " ";
                        });
                    }
                    else {
                        resultinfix.style.background = "#ad1f1f";
                        resultinfix.style.borderColor = "#451717";
                        resultinfix.innerHTML += "Błąd: " + data.message;
                    }
                }
                else {
                    resultinfix.style.background = "#ad1f1f";
                    resultinfix.style.borderColor = "#451717";
                    resultinfix.innerHTML = "Error: Zwrócony wynik jest błędny, przepraszamy za utrudnienia";
                }
            };
            request.send();
        }
        resultinfix.style.display = "block";
    }
}
else if (calculatexform != null) {

    const inputFormulaX = calculatexform.querySelector("textarea[name=formulax]");

    const inputX = calculatexform.querySelector("input[name=x]");

    const xresult = document.getElementById('xresult');

    inputX.onchange = checkX;
    function checkX() {
        allgoodx = true;
        inputX.style.color = "black";
        var x1 = /^\d+\.\d$|^\d$/i;


        if (!x1.test(inputX.value) || inputX.value == "") {
            inputX.style.color = "red";
            allgoodx = false;
        }
    }
    xresult.style.display = "none";
    calculatexform.onsubmit = function (e) {
        e.preventDefault();
        xresult.style.display = "none";
        if (inputFormulaX.value == "") {
            xresult.style.background = "#ad1f1f";
            xresult.style.borderColor = "#451717";
            xresult.innerHTML = "Błąd: Równanie jest puste";
        }
        else if (inputFormulaX.value.includes("=")) {
            xresult.style.background = "#ad1f1f";
            xresult.style.borderColor = "#451717";
            xresult.innerHTML = "Błąd: Równanie zawiera =";
        }
        else if (inputFormulaX.value.includes(" ")) {
            xresult.style.background = "#ad1f1f";
            xresult.style.borderColor = "#451717";
            xresult.innerHTML = "Błąd: Równanie zawiera spację";
        }
        else if (allgoodx) {
            var xValue = inputFormulaX.value;
            xValue = xValue.split("+").join("=");
            var request = new XMLHttpRequest();
            request.open('GET', link +  'calculate?formula=' + xValue + '&x=' + inputX.value, true);
            request.onerror = function () {
                alert("Połączenie z API zostało zerwane");
            }
            request.onload = function () {
                var data = JSON.parse(this.response);
                if (request.status >= 200 && request.status < 400) {
                    if (data.status == "ok") {
                        xresult.style.background = "#249424";
                        xresult.style.borderColor = "#0f4e0f";
                        xresult.innerHTML = "Wynik: " + data.result;

                    }
                    else {
                        xresult.style.background = "#ad1f1f";
                        xresult.style.borderColor = "#451717";
                        xresult.innerHTML = "Błąd: " + data.message;

                    }
                }
                else {
                    xresult.style.background = "#ad1f1f";
                    xresult.style.borderColor = "#451717";
                    xresult.innerHTML = "Error: Zwrócony wynik jest błędny, przepraszamy za utrudnienia";
                }
            };
            request.send();

        }
        else {
            xresult.style.background = "#ad1f1f";
            xresult.style.borderColor = "#451717";
            xresult.innerHTML = "Błąd: X jest pusty lub jego wartośc jest nie poprawna";
        }
        xresult.style.display = "block";
    }
}
else if (calculaterangeform != null) {

    const inputFormulaRange = calculaterangeform.querySelector("textarea[name=formularange]");

    const inputFrom = calculaterangeform.querySelector("input[name=from]");
    const inputTo = calculaterangeform.querySelector("input[name=to]");
    const inputN = calculaterangeform.querySelector("input[name=n]");

    const rangeresult = document.getElementById('rangeresult');
    const rangeresult1 = document.getElementById('rangeresult1');


    inputFrom.onchange = checkFrom;
    function checkFrom() {
        allgoodrange = true;
        inputFrom.style.color = "black";
        var x1 = /^\d+\.\d+$|^\d+$/i;

        if (!x1.test(inputFrom.value) || inputFrom.value == "") {
            inputFrom.style.color = "red";
            allgoodrange = false;
        }
    }

    inputTo.onchange = checkTo;
    function checkTo() {
        allgoodrange = true;
        inputTo.style.color = "black";
        var x1 = /^\d+\.\d+|\d+$/i;


        if (!x1.test(inputTo.value) || inputTo.value == "") {
            inputTo.style.color = "red";
            allgoodrange = false;
        }
    }

    inputN.onchange = checkN;
    function checkN() {
        allgoodrange = true;
        inputN.style.color = "black";

        var x1 = /^\d+$/i;

        if (!x1.test(inputN.value) || inputN.value == "") {
            inputN.style.color = "red";

            allgoodrange = false;
        }
    }
    rangeresult.style.display = "none";
    rangeresult1.style.display = "none";    1
    calculaterangeform.onsubmit = function (e) {

        e.preventDefault();
        rangeresult.style.display = "none";
        rangeresult1.style.display = "none";
        if (inputFormulaRange.value == "") {
            rangeresult.style.background = "#ad1f1f";
            rangeresult.style.borderColor = "#451717";
            rangeresult.innerHTML = "Błąd: Równanie jest puste";
            rangeresult.style.display = "block";
        }
        else if (inputFormulaRange.value.includes("=")) {
            rangeresult.style.background = "#ad1f1f";
            rangeresult.style.borderColor = "#451717";
            rangeresult.innerHTML = "Błąd: Równanie zawiera =";
            rangeresult.style.display = "block";
        }
        else if (inputFormulaRange.value.includes(" ")) {
            rangeresult.style.background = "#ad1f1f";
            rangeresult.style.borderColor = "#451717";
            rangeresult.innerHTML = "Błąd: Równanie zawiera spację";
            rangeresult.style.display = "block";
        }
        else if (allgoodrange) {
            var notincludet = 0, increment = 0;
            var rangeValue = inputFormulaRange.value;
            rangeValue = rangeValue.split("+").join("=");
            var request = new XMLHttpRequest();
            request.open('GET', link + 'calculate/xy?formula=' + rangeValue + '&from=' + inputFrom.value + '&to=' + inputTo.value + '&n=' + inputN.value, true);
            request.onerror = function () {
                alert("Połączenie z API zostało zerwane");
            }
            request.onload = function () {
                var data = JSON.parse(this.response);
                var results;

                if (request.status >= 200 && request.status < 400) {
                    if (data.status == "ok") {

                        results = data.result;

                        for (var i = 0; i < results.length; i++) {
                            if (results[i].y == "Wynik jest zbyt wysoki") {
                                notincludet += 1;
                                results.splice(i, 1);
                                i--;
                            }
                        }

                        if (notincludet != 0) {
                            rangeresult.style.background = "#ad1f1f";
                            rangeresult.style.borderColor = "#451717";
                            rangeresult.innerHTML = "Błąd: Nieudało się umieścić na wykresie " + notincludet + " wartości poniważ ich wyniki były zbyt wysokie ";
                            rangeresult.style.display = "block";
                        }
                        rangeresult1.style.display = "block";
                        RenderChart(results);

                    }
                    else {
                        rangeresult.style.background = "#ad1f1f";
                        rangeresult.style.borderColor = "#451717";
                        rangeresult.innerHTML = "Błąd: " + data.message;
                        rangeresult.style.display = "block";
                    }

                }
                else {
                    rangeresult.style.background = "#ad1f1f";
                    rangeresult.style.borderColor = "#451717";
                    rangeresult.innerHTML = "Error: Zwrócony wynik jest błędny, przepraszamy za utrudnienia";
                    rangeresult.style.display = "block";
                }
            }
            request.send();
        }
        else {
            rangeresult.style.background = "#ad1f1f";
            rangeresult.style.borderColor = "#451717";
            rangeresult.innerHTML = "Błąd: Któraś wartość jest pusta lub niepoprawna, wartości zaznaczone na czerwono są nie poprawne";
            rangeresult.style.display = "block";
        }

    }


}
else if (onpform != null) {

    const inputFormulaONP = onpform.querySelector("textarea[name=formulaonp]");

    const onpresult = document.getElementById('onpresult');

    onpresult.style.display = "none";

    onpform.onsubmit = function (e) {
        e.preventDefault();
        onpresult.style.display = "none";
        if (inputFormulaONP.value == "") {
            onpresult.style.background = "#ad1f1f";
            onpresult.style.borderColor = "#451717";
            onpresult.innerHTML = "Błąd: Nie podałeś żadnego elementu";

        }
        else if (inputFormulaONP.value.includes("=")) {
            onpresult.style.background = "#ad1f1f";
            onpresult.style.borderColor = "#451717";
            onpresult.innerHTML = "Błąd: Równanie zawiera =";

        }
        else {
            var oNPValue = inputFormulaONP.value;
            oNPValue = oNPValue.split("+").join("=");
            var request = new XMLHttpRequest();
            request.open('GET', link + 'reverse?onp=' + oNPValue, true);
            request.onerror = function () {
                alert("Połączenie z API zostało zerwane");
            }
            request.onload = function () {
                var data = JSON.parse(this.response);
                if (request.status >= 200 && request.status < 400) {

                    if (data.status == "ok") {
                        onpresult.style.background = "#249424";
                        onpresult.style.borderColor = "#0f4e0f";
                        onpresult.innerHTML = "Równanie: " + data.result;

                    }
                    else {
                        onpresult.style.background = "#ad1f1f";
                        onpresult.style.borderColor = "#451717";
                        onpresult.innerHTML = "Błąd: " + data.message;

                    }
                }
                else {

                    onpresult.style.background = "#ad1f1f";
                    onpresult.style.borderColor = "#451717";
                    onpresult.innerHTML = "Error: Zwrócony wynik jest błędny, przepraszamy za utrudnienia";

                }

            };
            request.send();
        }
        onpresult.style.display = "block";
    }

    function erase() {
        inputFormula.value = "";
        inputFormulaX.value = "";
        inputFormulaRange.value = "";
        inputFormulaONP.value = "";
        inputX.value = "";
        inputFrom.value = "";
        inputTo.value = "";
        inputN.value = "";
        resultinfix.innerHTML = "";
        resultonp.innerHTML = "";
        xresult.innerHTML = "";
        rangeresult.innerHTML = "";
        rangeresult1.innerHTML = "";
        onpresult.innerHTML = "";

        resultinfix.style.display = "none";
        resultonp.style.display = "none";
        xresult.style.display = "none";
        rangeresult.style.display = "none";
        rangeresult1.style.display = "none";
        onpresult.style.display = "none";
    }

   
}

function RenderChart(results) {

    var chart = new CanvasJS.Chart("rangeresult1",
        {
            animationEnabled: true,
            zoomEnabled: true,
            title: {
                text: "Wyniki obliczeń"
            },
            axisX: {
                valueFormatString: "#0.########",
                title: "Podane wartości",
            },
            axisY: {
                title: "Wyniki",
                includeZero: true,
            },
            legend: {
                cursor: "pointer",
                fontSize: 16,
                itemclick: toggleDataSeries
            },
            toolTip: {
                shared: true
            },
            data: [{
                name: "wynik",
                type: "spline",
                yValueFormatString: "#0.#############",
                showInLegend: true,
                dataPoints: results
            }]
        });
    chart.render();

    function toggleDataSeries(e) {
        if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
            e.dataSeries.visible = false;
        }
        else {
            e.dataSeries.visible = true;
        }
        chart.render();
    }

}

function TryConnection()
{
        var request = new XMLHttpRequest();
        request.onerror = function() { 
            alert("Nie udało się połączyć z API"); 
        }
        request.open('GET', link + 'values', true); 
        request.send();   
}


TryConnection();