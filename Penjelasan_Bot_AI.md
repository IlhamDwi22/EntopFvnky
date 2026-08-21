# Penjelasan Fitur AI Bot (FNFBotAI.cs)

Berdasarkan file `FNFBotAI.cs`, sistem AI (Bot) dalam game ritme (bertema Friday Night Funkin') ini memiliki beberapa fitur utama yang mengatur perilakunya. Berikut adalah penjelasan lengkapnya:

## 1. Sistem Tingkat Kesulitan Dinamis (Difficulty Scaling)
Fungsi `TerapkanDifficulty(int tingkatKesulitan)` mengatur seberapa jago bot bermain.
* Parameter `tingkatKesulitan` dimasukkan dalam skala 0 hingga 100.
* Sistem menggunakan rumus interpolasi linier (`Mathf.Lerp`) di mana akurasi bot (`botHitChance`) akan diskalakan antara **40% (paling rendah)** hingga **100% (paling tinggi/sempurna)**.
* Jika disetel ke 100, bot tidak akan pernah meleset. Jika disetel ke angka yang lebih rendah, bot kadang-kadang akan gagal mengenai panah (miss).

## 2. Evaluasi Not dan Probabilitas Pukulan (Hit/Miss)
Fungsi `EvaluateBotNote(FNFNoteController note)` adalah inti dari pengambilan keputusan AI setiap kali ada panah yang harus ia tekan.
* Bot akan mengacak angka dari 0 sampai 100 (`Random.Range(0f, 100f)`).
* **Jika Bot Berhasil (Angka Acak <= `botHitChance`)**:
  * Kombo bot bertambah.
  * Skor bot bertambah sebanyak **350 poin**.
  * Bot akan melakukan pose sesuai arah panah (Kiri, Bawah, Atas, atau Kanan).
* **Jika Bot Gagal/Miss (Angka Acak > `botHitChance`)**:
  * Kombo bot putus (kembali ke 0).
  * Skor bot dikurangi sebanyak **50 poin** (namun tidak bisa kurang dari 0).
  * Bot tidak melakukan pose keberhasilan.

## 3. Sistem Animasi dan Tahan Pose (Hold Note)
Animasi dikendalikan oleh fungsi `MainkanAnimasiBot` dan fungsi `Update()`.
* **Deteksi Arah**: Tergantung panah di *lane* mana (0: Kiri, 1: Bawah, 2: Atas, 3: Kanan), bot akan memainkan animasi arah yang bersangkutan.
* **Tahan Pose (Hold Notes)**: Saat bot memukul panah panjang (panah yang memiliki buntut/durasi), bot akan menahan posenya selama durasi panah tersebut menggunakan variabel `botIdleTimer`. Jika panah biasa, bot akan menahan pose selama 0.4 detik.
* **Kembali ke Idle**: Pada fungsi `Update()`, *timer* bot akan terus berkurang seiring berjalannya waktu permainan (`Time.deltaTime`). Begitu *timer* habis, bot akan otomatis kembali memutar animasi `"Idle"`.

## 4. Integrasi UI (Antarmuka Pengguna)
Bot ini secara otomatis akan memperbarui skor miliknya di layar menggunakan fungsi `UpdateBotUI()`.
* Fitur ini cukup fleksibel karena sudah mendukung dua tipe teks dalam Unity: teks UI standar dari Unity lama (`Text` legacy) maupun teks modern (`TextMeshProUGUI`). 

Secara keseluruhan, AI ini dirancang dengan sangat baik untuk sebuah game *rhythm*, lengkap dengan sistem acak yang masuk akal, kalkulasi skor mandiri, dan dukungan animasi untuk *hold notes* (panah panjang) yang membuat karakternya terasa lebih hidup.
