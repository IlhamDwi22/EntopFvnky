# EntopFvnky

EntopFvnky adalah game unik yang menggabungkan elemen eksplorasi platformer 2D (Overworld) dengan mekanisme pertarungan ritmik (Rhythm Battle) ala *Friday Night Funkin'*. Pemain dapat menjelajahi dunia, berinteraksi dengan NPC, dan menantang musuh-musuh dalam pertarungan ritme yang menantang!

## 🎮 Fitur Utama

### 1. Mode Eksplorasi (Overworld)
Pemain dapat bebas menjelajahi map 2D dengan mekanisme pergerakan yang mulus.
* **Pergerakan & Lompatan:** Karakter Utama (MC) dapat berlari dan melompat dengan dukungan sistem pendeteksi tanah (Ground Check) untuk mencegah lompatan ganda di udara.
* **Interaksi NPC & Dialog:** Berbicara dengan NPC di dunia. Sistem secara otomatis akan menghentikan pergerakan pemain agar fokus pada dialog.
* **Memicu Pertarungan:** Musuh tersebar di map. Dekati musuh dan tekan tombol `E` di dalam zona deteksi untuk masuk ke mode pertarungan.

### 2. Mode Pertarungan Ritme (Rhythm Battle)
Saat bertarung, game berubah menjadi mode ritmik (Friday Night Funkin' Style).
* **4 Jalur Panah (Lanes):** Kiri, Bawah, Atas, dan Kanan.
* **Sistem Skor & Akurasi:**
  * **SICK!!** (+350 Skor) untuk ketepatan sempurna.
  * **GOOD!** (+200 Skor) untuk ketepatan tinggi.
  * **BAD** (+50 Skor) untuk ketepatan yang kurang.
  * **MISS!** (-50 Penalti) jika terlewat, mereset combo.
* **Hold Notes:** Dukungan untuk menahan panah panjang dengan sistem pose tahan dan ekstra poin.
* **Tingkat Kesulitan Dinamis:** Rentang kesulitan menentukan ketatnya jendela waktu (hit window) akurasi pukulan.

### 3. Sistem Musuh (Enemy & Bot AI)
* **Data Musuh Berbasis ScriptableObject:** Setiap musuh memiliki "profil" tersendiri yang mengatur nama, animasi visual, tingkat kesulitan, dan *playlist* lagu yang harus diselesaikan pemain.
* **AI Bot Cerdas:** Musuh dikendalikan oleh AI dinamis yang akurasinya (40% - 100%) bergantung pada tingkat kesulitan yang disetel. AI memiliki perhitungan hit/miss tersendiri dan mengeksekusi animasi pose sesuai arah panah.

## 🛠️ Dibangun Menggunakan
* **Unity Engine** (C#)

## 🚀 Cara Bermain
1. Gunakan tombol panah / `WASD` untuk bergerak di mode Eksplorasi.
2. Gunakan `Spasi` / Panah Atas / `W` untuk melompat.
3. Dekati musuh dan tekan `E` untuk menantang mereka.
4. Di mode pertarungan, tekan tombol sesuai arah panah yang muncul di layar dengan tepat waktu!

---
*Game ini dikembangkan sebagai proyek pembelajaran dan eksplorasi mekanik game.*
