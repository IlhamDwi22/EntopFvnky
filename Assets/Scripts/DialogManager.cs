using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // Wajib ditambahkan untuk menggunakan System.Action

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    [Header("UI Component")]
    [Tooltip("Tarik GameObject Panel/Kotak Dialog dari Canvas ke sini")]
    public GameObject dialogPanel;
    
    [Tooltip("Tarik teks untuk Nama Karakter ke sini")]
    public Text textNamaLegacy;
    public TextMeshProUGUI textNamaTMP;

    [Tooltip("Tarik teks untuk Isi Dialog ke sini")]
    public Text textIsiLegacy;
    public TextMeshProUGUI textIsiTMP;

    private DialogData dataAktif;
    private int indeksPercakapanSaatIni;
    public bool sedangBicara { get; private set; } = false;

    // Menyimpan perintah/aksi apa yang harus dilakukan setelah dialog selesai
    private Action aksiSetelahDialog;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (dialogPanel != null) dialogPanel.SetActive(false);
    }

    private void Update()
    {
        if (!sedangBicara) return;

        // Tekan Spasi, Enter, atau Klik Kiri untuk lanjut ke teks berikutnya
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            TampilkanKalimatBerikutnya();
        }
    }

    // Fungsi MulaiDialog sekarang menerima parameter tambahan berupa aksi/perintah
    public void MulaiDialog(DialogData data, Action aksiSelesai = null)
    {
        if (data == null || data.daftarPercakapan.Length == 0)
        {
            Debug.LogWarning("[DialogManager] Data Dialog kosong atau tidak ditemukan!");
            return;
        }

        dataAktif = data;
        indeksPercakapanSaatIni = 0;
        sedangBicara = true;
        aksiSetelahDialog = aksiSelesai; // Simpan aksinya

        if (dialogPanel != null) dialogPanel.SetActive(true);

        UpdateTeksDialog();
    }

    private void TampilkanKalimatBerikutnya()
    {
        indeksPercakapanSaatIni++;

        if (indeksPercakapanSaatIni < dataAktif.daftarPercakapan.Length)
        {
            UpdateTeksDialog();
        }
        else
        {
            SelesaiDialog();
        }
    }

    private void UpdateTeksDialog()
    {
        Percakapan percakapanAktif = dataAktif.daftarPercakapan[indeksPercakapanSaatIni];
        
        if (textNamaLegacy != null) textNamaLegacy.text = percakapanAktif.namaPembicara;
        if (textNamaTMP != null) textNamaTMP.text = percakapanAktif.namaPembicara;

        if (textIsiLegacy != null) textIsiLegacy.text = percakapanAktif.kalimat;
        if (textIsiTMP != null) textIsiTMP.text = percakapanAktif.kalimat;
    }

    private void SelesaiDialog()
    {
        sedangBicara = false;
        if (dialogPanel != null) dialogPanel.SetActive(false);
        dataAktif = null;

        // Jika ada perintah/aksi setelah dialog, jalankan sekarang!
        if (aksiSetelahDialog != null)
        {
            aksiSetelahDialog.Invoke();
            aksiSetelahDialog = null; // Kosongkan kembali agar tidak terpanggil dua kali
        }
    }
}