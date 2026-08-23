# 🎵 EntopFvnky

> **Fight the beat. Reclaim the music.**

**EntopFvnky** adalah game **2D rhythm-action adventure** yang menggabungkan eksplorasi platformer dengan pertarungan berbasis ritme. Pemain berperan sebagai **Zero**, seorang musisi underground yang harus menyusuri dunia urban, membangun reputasi, dan menghadapi orang-orang yang berada di balik pencurian musiknya.

Alih-alih sekadar memilih lagu lalu memainkan rhythm game, EntopFvnky membawa mekanik ritme langsung ke dalam sebuah perjalanan. **Eksplorasi, interaksi, cerita, dan pertarungan semuanya menjadi bagian dari satu gameplay loop.**

---

## 🎮 Gameplay

Gameplay EntopFvnky terbagi menjadi dua pengalaman utama:

### 🌆 Overworld Exploration

Jelajahi lingkungan kota 2D dan temukan jalan menuju lawan berikutnya.

- 🏃 **Movement & Platforming** — Berlari dan melompat melewati lingkungan.
- 💬 **NPC & Dialogue** — Berinteraksi dengan karakter dan menemukan informasi sepanjang perjalanan.
- 🚪 **Interactive Environment** — Berinteraksi dengan berbagai objek untuk membuka jalan dan melanjutkan progres.
- ⚔️ **Battle Encounter** — Dekati musuh dan tekan `E` untuk memulai Rhythm Battle.

### 🎵 Rhythm Battle

Ketika pertarungan dimulai, permainan berubah menjadi duel ritme yang terinspirasi oleh game rhythm seperti _Friday Night Funkin'_.

Pemain harus mengikuti pola not yang muncul pada empat lane:

**← ↓ ↑ →**

Timing setiap input menentukan hasil serangan:

| Result     | Score | Effect          |
| ---------- | ----: | --------------- |
| **SICK!!** |  +350 | Perfect timing  |
| **GOOD!**  |  +200 | Accurate timing |
| **BAD**    |   +50 | Poor timing     |
| **MISS!**  |   -50 | Combo reset     |

Game juga mendukung **Hold Notes**, combo, akurasi, dan tingkat kesulitan yang memengaruhi ketatnya hit window.

> **Semakin baik ritmemu, semakin besar peluangmu untuk memenangkan pertarungan.**

---

## 🤖 Enemy & AI System

Setiap musuh memiliki karakteristik dan tingkat kesulitan tersendiri.

Data musuh disimpan menggunakan **Unity ScriptableObject**, sehingga profil musuh dapat mengatur berbagai parameter seperti:

- Nama dan identitas karakter
- Visual dan animasi
- Tingkat kesulitan
- Playlist / lagu pertarungan
- Tingkat akurasi AI

Lawan dikendalikan oleh **rhythm-based AI** yang memiliki peluang hit/miss berdasarkan tingkat kesulitan. AI juga merespons note dengan animasi pose yang sesuai dengan arah input.

Dengan demikian, lawan bukan hanya objek yang menunggu pemain selesai bermain, tetapi ikut **berpartisipasi dalam duel ritme**.

---

## 📖 Story

Zero adalah seorang musisi underground yang karyanya dicuri dan dimanfaatkan oleh pihak yang lebih besar.

Perjalanannya membawa Zero melewati berbagai lingkungan kota dan mempertemukannya dengan orang-orang yang memiliki hubungan dengan **Kingpin Records**.

Setiap kemenangan membawa Zero semakin dekat dengan sumber masalahnya.

```text
Underground
    ↓
Street Rapper
    ↓
Kingpin Records
    ↓
Executive Producer
    ↓
Idol Rapper
    ↓
Final Battle
```

Apa yang awalnya terlihat seperti sekadar perjalanan untuk mendapatkan kembali sebuah lagu perlahan berubah menjadi perjalanan untuk **membuktikan siapa pemilik sebenarnya dari musik tersebut.**

---

## ✨ Key Features

- 🎵 **Rhythm-based Combat**
- 🌆 **2D Urban Exploration**
- 🕹️ **Platforming & Movement**
- 💬 **NPC Dialogue System**
- 🤖 **Rhythm-based Enemy AI**
- 📊 **Score & Accuracy System**
- 🔥 **Combo System**
- 🎼 **Hold Notes**
- 📈 **Dynamic Difficulty**
- 📦 **ScriptableObject-based Enemy Data**
- 🎬 **Story-driven Level Progression**

---

## 🛠️ Built With

**Engine**

- Unity

**Programming**

- C#

**Core Systems**

- Unity Input System
- ScriptableObject
- 2D Physics
- Audio & Rhythm Synchronization
- Animation System

---

## 🎯 Game Flow

```text
        ┌──────────────┐
        │   Explore    │
        └──────┬───────┘
               ↓
        ┌──────────────┐
        │   Encounter  │
        │    Enemy     │
        └──────┬───────┘
               ↓
        ┌──────────────┐
        │ Rhythm Battle│
        └──────┬───────┘
               ↓
        ┌──────────────┐
        │     Win      │
        └──────┬───────┘
               ↓
        ┌──────────────┐
        │  Progression │
        └──────┬───────┘
               ↓
        ┌──────────────┐
        │ Next Area /  │
        │     Boss     │
        └──────────────┘
```

---

## 🎮 Controls

### Overworld

| Input               | Action                  |
| ------------------- | ----------------------- |
| `WASD` / Arrow Keys | Move                    |
| `Space` / `W` / `↑` | Jump                    |
| `E`                 | Interact / Start Battle |

### Rhythm Battle

| Input | Action     |
| ----- | ---------- |
| `←`   | Left Note  |
| `↓`   | Down Note  |
| `↑`   | Up Note    |
| `→`   | Right Note |

---

## 🧩 Development

EntopFvnky dikembangkan sebagai proyek game berbasis Unity dengan fokus pada eksperimen dan implementasi **rhythm-based combat dalam lingkungan 2D adventure**.

Proyek ini menjadi kesempatan untuk mengeksplorasi bagaimana sistem rhythm game dapat dikombinasikan dengan mekanik game yang lebih konvensional seperti:

- Character Controller
- Collision & Ground Detection
- Scene Management
- Dialogue & Interaction
- Enemy AI
- Animation State
- Audio Synchronization
- Rhythm / Beat Detection
- Score & Accuracy Calculation

Tujuan utamanya bukan hanya membuat rhythm game, tetapi menciptakan pengalaman di mana **ritme menjadi bagian dari dunia dan pertarungan itu sendiri.**

---

## 🚧 Project Status

**Status:** `Completed / Prototype`

EntopFvnky dikembangkan sebagai proyek pembelajaran dan eksplorasi mekanik game menggunakan Unity.

Beberapa sistem dibuat dengan pendekatan modular agar dapat dikembangkan dan diperluas pada tahap berikutnya.

---

## 👥 Team

| No. | Name | NIM | Role & Contribution |
| --- | --- | --- | --- |
| 1 | **Abiyyu Daffa Hidastya** | 3.34.24.1.01 | **Map & Environment** — Membuat map dan mencari aset credit scene. |
| 2 | **Ilham Dwipangga Sunarko Putra** | 3.34.24.1.10 | **Game Design & Documentation** — Membuat GDD, aset objek interaktif, dan membantu perancangan gameplay. |
| 3 | **Is\'ad Sabda Putra Mujiono** | 3.34.24.1.11 | **Unity Developer** — Mengembangkan sistem gameplay dan implementasi game di Unity. |
| 4 | **Maulana Azka Rifki S.** | 3.34.24.1.13 | **Asset & Audio** — Mencari dan membuat aset game serta menyesuaikan musik. |

---

## 📸 Screenshots

![Level 2](Docs/lvl_2.jpeg)
![Level 3](Docs/lvl_3.jpeg)

---

## 🎥 Gameplay

> _Gameplay video coming soon._

---

## 📄 Documentation

Dokumentasi desain game tersedia dalam **Game Design Document (GDD)** yang mencakup konsep, gameplay, karakter, level design, sistem permainan, hingga aspek teknis pengembangan.

---

## 💡 Why EntopFvnky?

EntopFvnky lahir dari sebuah pertanyaan sederhana:

> **"Bagaimana jika rhythm game tidak hanya dimainkan, tetapi menjadi cara utama untuk bertarung?"**

Dari pertanyaan tersebut, lahirlah sebuah game yang mencoba menggabungkan **beat, exploration, combat, dan story** ke dalam satu pengalaman.

**Feel the beat. Hit the notes. Reclaim the music. 🎵**
