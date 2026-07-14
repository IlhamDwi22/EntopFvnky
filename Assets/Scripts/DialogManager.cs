using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    [Header("Komponen UI (TextMeshPro)")]
    public GameObject panelDialog;
    public TextMeshProUGUI textIsi;
    public TextMeshProUGUI textNama; 

    [Header("Komponen UI (Legacy)")]
    public Text textIsiLegacy;
    public Text textNamaLegacy;

    [Header("Efek Animasi (Typewriter)")]
    [Tooltip("Waktu jeda antar huruf. Semakin kecil angkanya, semakin cepat ngetiknya.")]
    public float kecepatanKetik = 0.04f; 
    public AudioSource sumberSuara;
    public AudioClip suaraKetik;

    private DialogData dialogAktif;
    private int indexDialog = 0;
    private Action callbackSelesai;
    
    private bool sedangMengetik = false;
    public bool sedangBicara { get; private set; } = false;
    private Coroutine coroutineKetik;
    private string teksFullSaatIni = "";

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (panelDialog != null) panelDialog.SetActive(false);
    }

    public void MulaiDialog(DialogData data, Action onSelesai = null)
    {
        dialogAktif = data;
        callbackSelesai = onSelesai;
        indexDialog = 0;
        sedangBicara = true;

        if (panelDialog != null) panelDialog.SetActive(true);
        TampilkanBarisBerikutnya();
    }

    private void TampilkanBarisBerikutnya()
    {
        if (dialogAktif == null || dialogAktif.daftarPercakapan == null)
        {
            SelesaiDialog();
            return;
        }

        if (indexDialog < dialogAktif.daftarPercakapan.Length) 
        {
            // Ambil data percakapan saat ini
            Percakapan percakapanAktif = dialogAktif.daftarPercakapan[indexDialog];

            // Set nama pembicara jika komponen UI tersedia
            if (textNama != null)
            {
                textNama.text = percakapanAktif.namaPembicara;
            }
            if (textNamaLegacy != null)
            {
                textNamaLegacy.text = percakapanAktif.namaPembicara;
            }

            // Ambil teks lengkap dari memori
            teksFullSaatIni = percakapanAktif.kalimat;
            
            // Hentikan ketikan sebelumnya (jika ada error) lalu mulai ketikan baru
            if (coroutineKetik != null) StopCoroutine(coroutineKetik);
            coroutineKetik = StartCoroutine(AnimasiKetik(teksFullSaatIni));
        }
        else
        {
            SelesaiDialog();
        }
    }

    private IEnumerator AnimasiKetik(string teks)
    {
        sedangMengetik = true;
        if (textIsi != null) textIsi.text = ""; // Kosongkan teks di layar sebelum mulai ngetik
        if (textIsiLegacy != null) textIsiLegacy.text = "";

        string teksBerjalan = "";
        // Ubah kalimat menjadi daftar huruf, lalu munculkan satu per satu
        foreach (char huruf in teks.ToCharArray())
        {
            teksBerjalan += huruf;
            if (textIsi != null) textIsi.text = teksBerjalan;
            if (textIsiLegacy != null) textIsiLegacy.text = teksBerjalan;

            // Mainkan suara HANYA jika hurufnya bukan spasi kosong
            if (huruf != ' ' && sumberSuara != null && suaraKetik != null)
            {
                // Mengubah pitch/nada suara secara acak sedikit agar tidak terdengar monoton seperti robot
                sumberSuara.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
                sumberSuara.PlayOneShot(suaraKetik);
            }

            // Tunggu beberapa milidetik sebelum memunculkan huruf selanjutnya
            yield return new WaitForSeconds(kecepatanKetik);
        }

        sedangMengetik = false;
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        // Deteksi input pemain: Klik Kiri Mouse, tombol Spasi, atau E
        if (panelDialog != null && panelDialog.activeSelf && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E)))
        {
            if (sedangMengetik)
            {
                // JIKA TEKS SEDANG BERJALAN: Pemain ingin Skip/Percepat
                if (coroutineKetik != null) StopCoroutine(coroutineKetik);
                if (textIsi != null) textIsi.text = teksFullSaatIni; // Langsung tampilkan teks utuh
                if (textIsiLegacy != null) textIsiLegacy.text = teksFullSaatIni;
                sedangMengetik = false;
            }
            else
            {
                // JIKA TEKS SUDAH SELESAI DIKETIK: Lanjut ke dialog berikutnya
                indexDialog++;
                TampilkanBarisBerikutnya();
            }
        }
    }

    private void SelesaiDialog()
    {
        sedangBicara = false;
        if (panelDialog != null) panelDialog.SetActive(false);
        if (callbackSelesai != null) callbackSelesai.Invoke();
    }
}