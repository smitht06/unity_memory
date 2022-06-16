using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class SceneController : MonoBehaviour
{
    [SerializeField] private MemoryCard originalCard;
    [SerializeField] private Sprite[] images;
    [SerializeField] private TMP_Text scoreLabel;
    private MemoryCard firstRevealed;
    private MemoryCard secondRevealed;
    public int score = 0;

    public const int gridRows = 3;
    public const int gridCols = 4;
    public const float offsetX = 2f;
    public const float offsetY = 2.5f;

    // Start is called before the first frame update
    private void Start()
    {
        var numberOfImages = images.Length;

        var numbersList = new List<int>();

        for (var i = 0; i < numberOfImages; i++)
        {
            numbersList.Add(i);
            numbersList.Add(i);
        }

        /*for (int x = 0; x < numbersList.Count; x++)
        {
            print(numbersList[x]);
        }*/

        var numbers = numbersList.ToArray();
        var startPos = originalCard.transform.position;

        numbers = ShuffleArray(numbers);

        for (var i = 0; i < gridCols; i++)
        for (var j = 0; j < gridRows; j++)
        {
            MemoryCard card;
            if (i == 0 && j == 0)
                card = originalCard;
            else
                card = Instantiate(originalCard) as MemoryCard;

            var index = j * gridCols + i;

            var id = numbers[index];
            card.SetCard(id, images[id]);

            var posX = offsetX * i + startPos.x;
            var posY = -(offsetY * j) + startPos.y;
            card.transform.position = new Vector3(posX, posY, startPos.z);
        }
    }

    private int[] ShuffleArray(int[] numbers)
    {
        var newArray = numbers.Clone() as int[];
        for (var i = 0; i < newArray.Length; i++)
        {
            var tmp = newArray[i];
            var r = Random.Range(i, newArray.Length);
            newArray[i] = newArray[r];
            newArray[r] = tmp;
        }

        return newArray;
    }

    public bool canReveal => secondRevealed == null;

    public void CardRevealed(MemoryCard card)
    {
        if (firstRevealed == null)
        {
            firstRevealed = card;
        }
        else
        {
            secondRevealed = card;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        if (firstRevealed.Id == secondRevealed.Id)
        {
            score++;
            scoreLabel.text = $"Score: {score}";
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            firstRevealed.Unreveal();
            secondRevealed.Unreveal();
        }

        firstRevealed = null;
        secondRevealed = null;
    }

    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
    }
}