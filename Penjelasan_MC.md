Dalam game ini, mekanisme Karakter Utama (MC / Player) terbagi menjadi dua mode gameplay yang berbeda: Mode Eksplorasi (Overworld) dan Mode Pertarungan
Ritmik (Rhythm Battle).
Berikut adalah penjelasan lengkap dari masing-masing mekanismenya:

### 1. Mekanisme Eksplorasi (Berdasarkan PlayerOverworld.cs )

Mode ini digunakan saat pemain sedang menjelajahi dunia (seperti game platformer 2D).

• Pergerakan Dasar: Pemain bisa bergerak ke kiri dan ke kanan menggunakan tombol panah (Kiri/Kanan) atau A/D. Karakter akan otomatis memutar balik tubuhnya
(di-flip) menghadap ke arah ia berjalan.
• Mekanisme Melompat (Jump):
• Pemain bisa melompat menggunakan Spasi, tombol W, atau Panah Atas.
• Terdapat sistem pendeteksi tanah (Ground Check) di bawah kaki pemain sehingga pemain tidak bisa terbang (lompat berkali-kali di udara).
• Terdapat juga sistem cooldown lompatan singkat (0.25 detik) untuk mencegah pergerakan loncat yang tidak wajar.
• Integrasi Dialog NPC: Jika pemain sedang berinteraksi dengan NPC atau sistem dialog sedang aktif ( sedangBicara ), karakter akan dipaksa berhenti bergerak
dan kecepatan berjalannya direset ke nol agar pemain fokus membaca dialog.
• Animasi: Data kecepatan lari dan arah hadap secara otomatis dikirimkan ke komponen Animator untuk memicu animasi lari atau diam (idle).

### 2. Mekanisme Pertarungan Ritmik (Berdasarkan FNFScoring.cs )

Mode ini aktif saat game memasuki fase pertarungan ala Friday Night Funkin'.
• Input Panah (Lanes): Pemain harus menekan tombol (bisa diatur lewat KeyMappingManager atau menggunakan panah standar/WASD) yang mewakili 4 jalur: Kiri,
Bawah, Atas, dan Kanan.
• Jendela Akurasi Waktu (Hit Windows): Ketepatan waktu pemain dalam menekan tombol akan dinilai dalam beberapa tingkatan:
• SICK!! (Skor +350): Jika ditekan sangat akurat dan presisi (selisih waktu < 0.05 detik).
• GOOD! (Skor +200): Jika ditekan cukup akurat (selisih waktu < 0.1 detik).
• BAD (Skor +50): Jika ditekan kurang akurat tetapi tidak meleset (selisih waktu < 0.15 detik).
• MISS! (Penalti Skor -50): Jika gagal menekan, menekan di saat kosong, atau melepas tombol terlalu cepat. Kombo akan reset ke 0.
• Panah Panjang (Hold Notes): Game ini mendukung hold notes. Pemain harus menahan tombol panah saat ada panah berbuntut dan melepasnya di saat yang tepat.
Jika dilepas di tengah jalan (lebih dari 0.1 detik sebelum buntut habis), pemain akan terkena "Miss". Menahan dengan benar akan memberikan ekstra skor (1
poin terus-menerus dan 100 poin saat selesai).
• Umpan Balik Visual (Receptors): Tombol target (Receptor) di layar akan membesar dan berubah warna sesaat ketika pemain menekan panah, dan warnanya
disesuaikan dengan akurasi pukulan (Misal: merah untuk miss, biru/cyan untuk sick).
• Tingkat Kesulitan Dinamis: Pemain juga bisa disesuaikan tingkat kesulitannya yang mana semakin sulit, maka "jendela waktu" (hit window) untuk mendapatkan
"Sick" atau "Good" akan semakin sempit/ketat, dan penalti poin saat "Miss" akan semakin besar (hingga minus 150 poin).
• Animasi Pertarungan: Setiap panah yang ditekan dengan benar akan membuat MC melakukan animasi pose (Kiri, Kanan, Atas, Bawah) sesuai arah yang ditekan,
dan jika ditahan (Hold), posenya akan dipertahankan.
