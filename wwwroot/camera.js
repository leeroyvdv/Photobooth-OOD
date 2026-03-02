let stream = null;
let lastSnapshot = null;

console.log("CAMERA JS LOADED V3");

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
function takeSnapshot() {
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

    runValidation(canvas);

    setStatus("📸 Foto genomen");
    setStep("step2");
}

/* ===============================
   UPLOAD
================================ */
function uploadPhoto(event) {
    const file = event.target.files[0];
    if (!file) return;

    const reader = new FileReader();

    reader.onload = function (e) {
        const img = document.getElementById("snapshot");
        img.src = e.target.result;
        img.style.display = "block";

        const canvas = document.createElement("canvas");
        const image = new Image();

        image.onload = function () {
            canvas.width = image.width;
            canvas.height = image.height;

            const ctx = canvas.getContext("2d");
            ctx.drawImage(image, 0, 0);

            lastSnapshot = canvas;

            runValidation(canvas);

            setStatus("📁 Foto geüpload");
            setStep("step2");
        };

        image.src = e.target.result;
    };

    reader.readAsDataURL(file);
}

/* ===============================
   VALIDATIE
================================ */
function runValidation(canvas) {

    const list = document.getElementById("validationList");
    if (!list) return;

    list.innerHTML = "";

    const ctx = canvas.getContext("2d");
    const { width, height } = canvas;

    function analyze(data) {
        let brightness = 0;
        let contrast = 0;

        for (let i = 0; i < data.data.length; i += 4) {
            const avg = (data.data[i] + data.data[i + 1] + data.data[i + 2]) / 3;
            brightness += avg;
            contrast += Math.abs(avg - 128);
        }

        const pixels = data.data.length / 4;

        return {
            brightness: brightness / pixels,
            contrast: contrast / pixels
        };
    }

    const center = ctx.getImageData(
        width * 0.3,
        height * 0.25,
        width * 0.4,
        height * 0.5
    );

    const left = ctx.getImageData(
        width * 0.2,
        height * 0.25,
        width * 0.15,
        height * 0.5
    );

    const right = ctx.getImageData(
        width * 0.65,
        height * 0.25,
        width * 0.15,
        height * 0.5
    );

    const full = ctx.getImageData(
        width * 0.1,
        height * 0.1,
        width * 0.8,
        height * 0.8
    );

    const c = analyze(center);
    const l = analyze(left);
    const r = analyze(right);
    const f = analyze(full);

    const symmetryDiff = Math.abs(l.contrast - r.contrast);

    const faceDetected = c.contrast > 18;
    const faceCentered = faceDetected && symmetryDiff < 20;
    const lightingOk = f.brightness > 50;
    const eyesVisible = faceDetected && faceCentered && c.brightness > 55;

    list.innerHTML += faceDetected
        ? "<li>✅ Gezicht gedetecteerd</li>"
        : "<li>❌ Geen gezicht gedetecteerd</li>";

    list.innerHTML += faceCentered
        ? "<li>✅ Gezicht in het midden</li>"
        : "<li>❌ Gezicht niet in het midden</li>";

    list.innerHTML += eyesVisible
        ? "<li>✅ Ogen goed zichtbaar</li>"
        : "<li>❌ Ogen niet goed zichtbaar</li>";

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