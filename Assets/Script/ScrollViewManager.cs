using UnityEngine;
using UnityEngine.UI;

public class ScrollViewManager : MonoBehaviour
{
    public GameObject content; // GameObject Content di ScrollView
    public GameObject itemPrefab; // Prefab UI yang akan menjadi child di dalam Content
    public int itemCount = 10; // Jumlah elemen yang ingin ditampilkan
    public float itemHeight = 100f; // Tinggi dari setiap item

    void Start()
    {
        PopulateScrollView();
    }

    void PopulateScrollView()
    {
        // Mengatur tinggi Content sesuai dengan jumlah item
        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, itemCount * itemHeight);

        // Menambahkan item ke dalam Content secara dinamis
        for (int i = 0; i < itemCount; i++)
        {
            // Instantiate prefab item
            GameObject newItem = Instantiate(itemPrefab, content.transform);

            // Atur posisi item dalam Content
            RectTransform itemRect = newItem.GetComponent<RectTransform>();
            itemRect.anchoredPosition = new Vector2(0, -i * itemHeight);
            itemRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, itemHeight);

            // Tambahkan teks pada item untuk membedakan
            newItem.GetComponentInChildren<Text>().text = "Item " + (i + 1);
        }
    }
}
