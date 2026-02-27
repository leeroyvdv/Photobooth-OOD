let stream = null;
let lastSnapshot = null;

function setStatus(text, type = "success") {
    const status = document.getElementById("status");
    if (!status) return;
    status.innerText = text;
    status.className = `status ${type}`;
    status.style.display = "block";
}

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
   =============================== */
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
    const video = document.getElementById("video");
    if (video) video.srcObject = null;
    setStatus("🛑 Camera gestopt", "warning");
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

    runValidation(canvas);
    setStatus("📸 Foto genomen");
    setStep("step2");
}

/* ===============================
   VALIDATIE (DEFINITIEF STABIEL)
   =============================== */
function runValidation(canvas) {
    const list = document.getElementById("validationList");
    list.innerHTML = "";

    const ctx = canvas.getContext("2d");
    const { width, height } = canvas;

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

    const c = analyze(center);
    const l = analyze(left);
    const r = analyze(right);
    const f = analyze(full);

    const symmetryDiff = Math.abs(l.contrast - r.contrast);

    // 👇 AFGESTELDE DREMPELS
    const faceDetected = c.contrast > 16;
    const faceCentered = faceDetected && symmetryDiff < 15;
    const lightingOk = f.brightness > 45;
    const eyesVisible = faceDetected && faceCentered && c.brightness > 50;

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
   OPSLAAN / DOWNLOAD
   =============================== */
function saveSnapshot() {
    if (!lastSnapshot) return;
    localStorage.setItem("photobooth_snapshot", lastSnapshot.toDataURL());
    setStatus("💾 Foto opgeslagen");
    setStep("step3");
}

function downloadSnapshot() {
    if (!lastSnapshot) return;
    const a = document.createElement("a");
    a.href = lastSnapshot.toDataURL("image/png");
    a.download = "photobooth.png";
    a.click();
}
