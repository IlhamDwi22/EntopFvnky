Sama halnya dengan MC, mekanisme musuh (Enemy) dalam game ini juga terbagi menjadi dua fase: Fase Overworld (Eksplorasi) dan Fase Pertarungan (Rhythm
Battle).

Berikut adalah penjelasan lengkapnya berdasarkan file EnemyNPC.cs dan OpponentData.cs :

### 1. Mekanisme NPC di Overworld (Berdasarkan EnemyNPC.cs )

Di mode penjelajahan, musuh bertindak layaknya NPC biasa yang menunggu untuk ditantang.
• Area Deteksi (Trigger Area): Musuh memiliki zona deteksi di sekitarnya. Jika karakter pemain (yang memiliki tag "Player") masuk ke zona tersebut, musuh
akan memunculkan sebuah UI Visual (seperti teks di atas kepala: "Tekan E untuk Bertarung").
• Memicu Pertarungan (Triggering Battle): Jika pemain sudah berada di dekat musuh dan menekan Tombol E, fungsi MulaiPertarungan() akan dieksekusi.
• Jembatan Data ke Mode Battle: Saat pertarungan dimulai, NPC ini akan mengambil Data Profil Musuh miliknya lalu menitipkannya ke memori global (
GameManager.musuhPilihanSaatIni ). Setelah itu, urutan lagu di-reset ke lagu pertama, dan game akan memuat ( loading) scene pertarungan ritme (secara
default bernama "Gameplay").

### 2. Struktur Data Musuh (Berdasarkan OpponentData.cs )

Game ini menggunakan fitur ScriptableObject dari Unity, yang artinya data musuh dibuat seperti sebuah "kartu profil" yang bisa diatur dengan mudah tanpa
menyentuh coding. Profil musuh ini menyimpan:
• Nama Musuh & Visual: Menyimpan nama musuh dan gambar karakternya.
• Controller Animasi ( animasiMusuh ): Setiap musuh bisa memiliki set animasinya sendiri-sendiri saat di arena (animasi kiri, kanan, atas, bawah, idle).
• Tingkat Kesulitan ( tingkatKesulitan ): Ada skala dari 0 hingga 100. Skala inilah yang nantinya akan dibaca oleh sistem AI bot ( FNFBotAI.cs ) untuk
menentukan seberapa sering bot gagal/meleset dalam menekan panah.
• Daftar Lagu ( daftarLagu ): Sebuah musuh tidak hanya menyajikan 1 lagu, tapi mereka bisa menyimpan kumpulan/daftar putar lagu (playlist) yang harus
diselesaikan pemain secara berurutan.

### 3. Fase Pertarungan / AI (Berdasarkan FNFBotAI.cs )

(Mekanisme ini sempat saya buatkan markdown-nya sebelumnya).
Saat berada di arena pertarungan, musuh dikendalikan sepenuhnya oleh AI:

• Ia akan menarik angka tingkat kesulitan (0-100) dari profil OpponentData di atas.
• Semakin tinggi kesulitannya, semakin tinggi peluangnya (sampai 100%) untuk mengenai not panah secara sempurna.
• Jika ia mengenai panah, ia akan mendapatkan skor (+350), menambah kombo, dan memicu animasi gaya sesuai dengan panah yang ia tekan (Kiri/Kanan/Atas/Bawah).
Ia juga bisa menahan gaya tersebut (hold pose) jika menekan panah berdurasi panjang.
