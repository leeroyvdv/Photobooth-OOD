let stream = null;
let lastSnapshot = null;

console.log("CAMERA JS LOADED V2");

/* ===============================
   HELPER: STATUS
   =============================== */
function setStatus(text, type = "success") {
    const status = document.getElementById("status");
    if (!status) return;
    status.innerText = text;
    status.className = `status ${type}`;
    status.style.display = "block";
}

/* ===============================
   CAMERA
   =============================== */
async function startCameraHard() {
    try {
        stream = await navigator.mediaDevices.getUserMedia({ video: true });
        const video = document.getElementById("video");
        video.srcObject = stream;
        video.play();
        setStatus("📷 Camera gestart");
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
   =============================== */
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

    setStatus("📸 Foto genomen");
}

/* ===============================
   UPLOAD VAN BESTAND
   =============================== */
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
            setStatus("📁 Foto geüpload");
        };

        image.src = e.target.result;
    };

    reader.readAsDataURL(file);
}

/* ===============================
   OPSLAAN NAAR BACKEND
   =============================== */
async function saveSnapshot() {
    if (!lastSnapshot) {
        setStatus("❌ Geen foto beschikbaar", "error");
        return;
    }

    const base64 = lastSnapshot
        .toDataURL("image/png")
        .split(',')[1];

    try {
        const response = await fetch("/api/validate-photo", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                base64Image: base64
            })
        });

        if (!response.ok) {
            setStatus("❌ Opslaan mislukt", "error");
            return;
        }

        const result = await response.json();

        setStatus(`💾 Foto opgeslagen (ID: ${result.savedPhotoId})`);
    } catch (err) {
        setStatus("❌ Server fout", "error");
    }
}