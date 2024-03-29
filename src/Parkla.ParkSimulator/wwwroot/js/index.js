﻿var randomIID;
var server = "https://localhost:7072"

const EMPTY_STATUS = "EMPTY";
const OCCUPIED_STATUS = "OCCUPIED";
const UNKNOWN_STATUS = "UNKNOWN";
const HTTP_PROTOCOL = "HTTP";
const GRPC_PROTOCOL = "GRPC";
const SERIAL_PROTOCOL = "SERIAL";

var spaces;

function setHeaders(request) {
    request.setRequestHeader("Content-Type", "application/json");
    request.setRequestHeader("Accept", "*/*");
}

function setElemStatus(elem, status) {
    if(status == EMPTY_STATUS) {
        elem.classList.remove("space-occupied");
        elem.classList.add("space-empty");        
    }
    else if(status == OCCUPIED_STATUS) {
        elem.classList.remove("space-empty");
        elem.classList.add("space-occupied");
    }
    else {
        elem.classList.remove("space-empty");
        elem.classList.remove("space-occupied");
    }
}

function receiveStatus(req) {
    const dto = JSON.parse(req.responseText);
    
    let elem
    for (let i = 0; i < spaces.length; i++) {
        const element = spaces[i];
        if(element.dataset.id == dto.spaceId) {
            elem = element;
            break;
        }
    }
    if(elem)
        setElemStatus(elem, dto.status);
}

function SetRandom() {
    const req = new XMLHttpRequest();
    req.open("GET", server+"/Space/Random");
    setHeaders(req);

    req.send();
    req.addEventListener("load", () => receiveStatus(req));
}

function setStatusRequest(elem, status) {
    const req = new XMLHttpRequest();
    req.open( "POST", server+"/Space/SetStatus");
    setHeaders(req);

    let data = JSON.stringify({
        spaceId: elem.dataset.id,
        status: status
    });
    req.send(data);

    req.addEventListener("load", () => receiveStatus(req));
}


function spaceClick(event) {
    let elem = event.target;
    if(elem.classList.contains("space-empty")) {
        setStatusRequest(elem, OCCUPIED_STATUS);
    }
    else if(elem.classList.contains("space-occupied")) {
        setStatusRequest(elem, EMPTY_STATUS);
    }
    else {
        setStatusRequest(elem, EMPTY_STATUS);
    }
}

function randomClick(event) {
    let elem = event.target;
    if(randomIID) {
        elem.innerHTML = "Start Simulation"
        clearInterval(randomIID);
        randomIID = undefined;
    }
    else {
        elem.innerHTML = "Stop Simulation"
        randomIID = setInterval(() => {
            SetRandom();
        }, 100);
    }
}

function setProtocol(protocol) {
    const req = new XMLHttpRequest();
    req.open( "GET", server+`/SetProtocol?protocol=${protocol}`);
    setHeaders(req);

    req.send();

    req.addEventListener("load", () => {
       document.querySelector("#protocolText").innerHTML = protocol;
    });
}

(function () {
    const serverResetReq = new XMLHttpRequest();
    serverResetReq.open("GET", server+"/ResetServer");
    setHeaders(serverResetReq);
    serverResetReq.send();

    spaces = Array.from(document.querySelectorAll(".space"));
    spaces.forEach(elem => {
        elem.addEventListener("click", spaceClick);
    })

    document.querySelector("#simulateButton").addEventListener("click", randomClick);
    document.querySelector("#httpProtocolButton").addEventListener("click", () => setProtocol(HTTP_PROTOCOL));
    document.querySelector("#grpcProtocolButton").addEventListener("click", () => setProtocol(GRPC_PROTOCOL));
    document.querySelector("#serialProtocolButton").addEventListener("click", () => setProtocol(SERIAL_PROTOCOL));
})();