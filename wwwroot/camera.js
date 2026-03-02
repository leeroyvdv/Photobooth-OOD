let stream = null;
let lastSnapshot = null;
let modelsLoaded = false;

console.log("CAMERA JS AI VERSION LOADED");

/* ===============================
   MODEL LOAD
================================ */
async function loadModels() {
    await faceapi.nets.tinyFaceDetector.loadFromUri('/models');
    await faceapi.nets.faceLandmark68Net.loadFromUri('/models');
    modelsLoaded = true;
    console.log("Face-api models loaded");
}

loadModels();

/* ===============================
   STATUS
================================ */
function setStatus(text, type = "success") {
    const status = document.getElementById("status");
    if (!status) return;
    status.innerText = text;
    status.className = `status ${type}`;
    status.style.display = "block";
}

/* ===============================
   STEP CONTROL
================================ */
function setStep(step) {
    ["step1", "step2", "step3"].forEach(id => {
        const el = document.getElementById(id);
        if (el) el.classList.remove("active");
    });

    const active = document.getElementById(step);
    if (active) active.classList.add("active");
}

/* ===============================
   CAMERA
================================ */
async function startCameraHard() {
    try {
        stream = await navigator.mediaDevices.getUserMedia({ video: true });
        const video = document.getElementById("video");
        video.srcObject = stream;
        video.play();
        setStatus("📷 Camera gestart");
        setStep("step1");
    } catch {
        setStatus("❌ Camera toegang geweigerd", "error");
    }
}

function stopCamera() {
    if (stream) {
        stream.getTracks().forEach(t => t.stop());
        stream = null;
    }
}

/* ===============================
   SNAPSHOT
================================ */
async function takeSnapshot() {
    const video = document.getElementById("video");
    if (!video || !video.srcObject) {
        setStatus("❌ Camera niet actief", "error");
        return;
    }

    const canvas = document.createElement("canvas");
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;

    const ctx = canvas.getContext("2d");
    ctx.drawImage(video, 0, 0);

    lastSnapshot = canvas;

    const img = document.getElementById("snapshot");
    img.src = canvas.toDataURL("image/png");
    img.style.display = "block";

    await runValidation(canvas);

    setStatus("📸 Foto genomen");
    setStep("step2");
}

/* ===============================
   UPLOAD
================================ */
async function uploadPhoto(event) {
    const file = event.target.files[0];
    if (!file) return;

    const reader = new FileReader();

    reader.onload = async function (e) {
        const img = document.getElementById("snapshot");
        img.src = e.target.result;
        img.style.display = "block";

        const canvas = document.createElement("canvas");
        const image = new Image();

        image.onload = async function () {
            canvas.width = image.width;
            canvas.height = image.height;

            const ctx = canvas.getContext("2d");
            ctx.drawImage(image, 0, 0);

            lastSnapshot = canvas;

            await runValidation(canvas);

            setStatus("📁 Foto geüpload");
            setStep("step2");
        };

        image.src = e.target.result;
    };

    reader.readAsDataURL(file);
}

/* ===============================
   AI VALIDATION
================================ */
async function runValidation(canvas) {

    const list = document.getElementById("validationList");
    if (!list) return;

    list.innerHTML = "";

    if (!modelsLoaded) {
        list.innerHTML += "<li>⏳ AI modellen laden...</li>";
        return;
    }

    const detection = await faceapi
        .detectSingleFace(canvas, new faceapi.TinyFaceDetectorOptions())
        .withFaceLandmarks();

    const ctx = canvas.getContext("2d");
    const { width, height } = canvas;

    let faceDetected = false;
    let faceCentered = false;
    let eyesVisible = false;
    let lightingOk = false;

    if (detection) {
        faceDetected = true;

        const box = detection.detection.box;
        const centerX = box.x + box.width / 2;
        const imageCenterX = width / 2;

        faceCentered = Math.abs(centerX - imageCenterX) < width * 0.15;

        const leftEye = detection.landmarks.getLeftEye();
        const rightEye = detection.landmarks.getRightEye();

        eyesVisible = leftEye.length > 0 && rightEye.length > 0;
    }

    // Lighting check
    const imageData = ctx.getImageData(0, 0, width, height);
    let brightness = 0;

    for (let i = 0; i < imageData.data.length; i += 4) {
        brightness += (imageData.data[i] +
            imageData.data[i + 1] +
            imageData.data[i + 2]) / 3;
    }

    brightness /= (imageData.data.length / 4);
    lightingOk = brightness > 60;

    list.innerHTML += faceDetected
        ? "<li>✅ Gezicht gedetecteerd (AI)</li>"
        : "<li>❌ Geen gezicht gedetecteerd</li>";

    list.innerHTML += faceCentered
        ? "<li>✅ Gezicht in het midden</li>"
        : "<li>❌ Gezicht niet in het midden</li>";

    list.innerHTML += eyesVisible
        ? "<li>✅ Ogen gedetecteerd</li>"
        : "<li>❌ Ogen niet gedetecteerd</li>";

    list.innerHTML += lightingOk
        ? "<li>✅ Belichting in orde</li>"
        : "<li>❌ Belichting onvoldoende</li>";
}

/* ===============================
   DOWNLOAD
================================ */
function downloadSnapshot() {
    if (!lastSnapshot) return;

    const a = document.createElement("a");
    a.href = lastSnapshot.toDataURL("image/png");
    a.download = "photobooth.png";
    a.click();
}

/* ===============================
   SAVE BACKEND
================================ */
async function saveSnapshot() {
    if (!lastSnapshot) {
        setStatus("❌ Geen foto beschikbaar", "error");
        return;
    }

    const base64 = lastSnapshot.toDataURL("image/png").split(',')[1];

    try {
        const response = await fetch("/api/validate-photo", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ base64Image: base64 })
        });

        if (!response.ok) {
            setStatus("❌ Opslaan mislukt", "error");
            return;
        }

        const result = await response.json();

        setStatus(`💾 Foto opgeslagen (ID: ${result.savedPhotoId})`);
        setStep("step3");

    } catch {
        setStatus("❌ Server fout", "error");
    }
}