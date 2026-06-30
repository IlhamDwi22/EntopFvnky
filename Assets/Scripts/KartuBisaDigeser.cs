using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class KartuBisaDigeser : MonoBehaviour
{
    private string myItemId;
    private Collider2D scannerTarget;
    private Vector3 offsetPosisi;

    // Dipanggil otomatis oleh Mesin untuk mengatur wujud barang
    public void SetupKartu(string id, Sprite ikon, Collider2D scanner)
    {
        myItemId = id;
        scannerTarget = scanner;
        GetComponent<SpriteRenderer>().sprite = ikon;
    }

    private void OnMouseDown()
    {
        // Menghitung selisih agar saat diseret, gambarnya tidak melompat mendadak ke tengah kursor
        offsetPosisi = transform.position - DapatkanPosisiMouse();
    }

    private void OnMouseDrag()
    {
        // Memindahkan posisi benda mengikuti posisi mouse saat ditahan
        transform.position = DapatkanPosisiMouse() + offsetPosisi;
    }

    private void OnMouseUp()
    {
        // Saat klik dilepas, kita cek apakah titik tengah benda ini menyentuh kotak scanner
        if (scannerTarget != null && scannerTarget.OverlapPoint(transform.position))
        {
            if (MesinGesekKartu.Instance != null)
            {
                MesinGesekKartu.Instance.VerifikasiGesekan(myItemId);
            }
        }
    }

    // Fungsi pembantu untuk menerjemahkan posisi layar (kursor) menjadi posisi dunia 2D
    private Vector3 DapatkanPosisiMouse()
    {
        Vector3 posisiMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        posisiMouse.z = 0f; // Wajib 0 untuk game 2D
        return posisiMouse;
    }
}