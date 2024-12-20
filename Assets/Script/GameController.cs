using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject[] Number;
    private readonly int width = 6;
    private readonly int height = 5;
    public GameObject[,] numArray = new GameObject[6, 5];
    private int next;
    private int plusNum;
    private GameObject activeNumber;
    private GameObject nextNumber;
    private int hitObjectNum;
    private GameObject hitObject;
    private Vector2 touchStartPos;
    private float touchHoldTime = 0f;
    private const float moveInterval = 0.2f;
    private float countup = 0.0f;

    public GameObject conText;
    public GameObject timeText;
    public GameObject clearText;
    public GameObject menuButton;

    void Start()
    {
        conText.SetActive(false);
        clearText.SetActive(false);
        menuButton.SetActive(false);
        CreateNumber();
        next = CreateOneNumber();
        CreateNowNumber(next);
        StartCoroutine(UpdateFallingBlocksCoroutine());
    }

    void Update()
    {
        HandleInput();
        CheckOBJ();
        countup += Time.deltaTime;
        timeText.GetComponent<TextMeshProUGUI>().text = countup.ToString("f1");
    }

    void CreateNumber()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int r = Random.Range(0, 10);
                var number = Instantiate(Number[r]);
                number.transform.position = new Vector2(x, y);
                numArray[x, y] = number;
            }
        }
    }

    void CreateNowNumber(int Next)
    {
        var number = Instantiate(Number[Next]);
        number.transform.position = new Vector2(3.0f, 13.0f);
        next = CreateOneNumber();
        var number2 = Instantiate(Number[next]);
        number2.transform.position = new Vector2(6.0f, 14.5f);

        activeNumber = number;
        nextNumber = number2;
        ColorUnder();
        StartCoroutine(DropObject(number));
    }

    int CreateOneNumber()
    {
        return Random.Range(0, 10);
    }

    IEnumerator DropObject(GameObject obj)
    {
        while (obj != null)
        {
            yield return new WaitForSeconds(0.8f);
            if (obj == null) break;

            obj.transform.position += new Vector3(0, -1.0f, 0);

            if (CheckCollision(obj.transform.position))
            {
                StartCoroutine(OnPlusNumber(obj));
                break;
            }

            if (obj.transform.position.y <= 0)
            {
                obj.transform.position = new Vector3((int)obj.transform.position.x, 0, 0);

                numArray[(int)obj.transform.position.x, 0] = obj;

                CheckNextTo(obj);

                Destroy(nextNumber);
                activeNumber = null;
                nextNumber = null;
                CreateNowNumber(next);
                break;
            }

        }
    }

    void HandleInput()
    {
        if (activeNumber != null)
        {
            Vector3 currentPosition = activeNumber.transform.position;

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchStartPos = touch.position;
                        touchHoldTime = 0f;
                        break;

                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        touchHoldTime += Time.deltaTime;

                        Vector2 swipeDelta = touch.position - touchStartPos;

                        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                        {
                            if (swipeDelta.x > 50 && currentPosition.x < 5)
                            {
                                activeNumber.transform.position += new Vector3(1, 0, 0);
                                touchStartPos = touch.position;
                                touchHoldTime = 0f;
                                ColorUnder();
                            }
                            else if (swipeDelta.x < -50 && currentPosition.x > 0)
                            {
                                activeNumber.transform.position += new Vector3(-1, 0, 0);
                                touchStartPos = touch.position;
                                touchHoldTime = 0f;
                                ColorUnder();
                            }
                        }
                        else if (Mathf.Abs(swipeDelta.y) > Mathf.Abs(swipeDelta.x) && swipeDelta.y < -200)
                        {
                            FastDrop();
                            touchStartPos = touch.position;
                            touchHoldTime = 0f;
                            ColorUnder();
                        }
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        touchHoldTime = 0f;
                        break;
                }
            }
            if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && currentPosition.x > 0)
            {
                activeNumber.transform.position += new Vector3(-1, 0, 0);
                ColorUnder();
            }
            if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && currentPosition.x < 5)
            {
                activeNumber.transform.position += new Vector3(1, 0, 0);
                ColorUnder();
            }
            if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && currentPosition.y > 0)
            {
                FastDrop();
                ColorUnder();
            }
        }
    }

    void ColorUnder()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (numArray[x, y] != null)
                {
                    numArray[x, y].GetComponent<SpriteRenderer>().color = new Color(244f / 255f, 245f / 255f, 238f / 255f);
                }
            }
        }

        for (int i = 0; i < 6; i++)
        {
            if (numArray[(int)activeNumber.transform.position.x, 0] == null)
            {
                //ColorNextTo(null);
                break;
            }
            else if (i < 5 && numArray[(int)activeNumber.transform.position.x, i] == null)
            {
                numArray[(int)activeNumber.transform.position.x, i - 1].GetComponent<SpriteRenderer>().color = new Color(240f / 255f, 86f / 255f, 123f / 255f);
                ColorNextTo(numArray[(int)activeNumber.transform.position.x, i - 1]);
                break;
            }
            else if (i == 5)
            {
                numArray[(int)activeNumber.transform.position.x, 4].GetComponent<SpriteRenderer>().color = new Color(240f / 255f, 86f / 255f, 123f / 255f);
                ColorNextTo(numArray[(int)activeNumber.transform.position.x, 4]);
                break;
            }
        }
    }

    void FastDrop()
    {
        for (int i = 0; i < 6; i++)
        {
            if (numArray[(int)activeNumber.transform.position.x, 0] == null)
            {
                activeNumber.transform.position = new Vector3((int)activeNumber.transform.position.x, 0, 0);
                break;
            }
            else if (i < 5 && numArray[(int)activeNumber.transform.position.x, i] == null)
            {
                hitObjectNum = numArray[(int)activeNumber.transform.position.x, i - 1].gameObject.GetComponent<SpriteNum>().num;
                hitObject = numArray[(int)activeNumber.transform.position.x, i - 1];
                StartCoroutine(OnPlusNumber(activeNumber));
                break;
            }
            else if (i == 5)
            {
                hitObjectNum = numArray[(int)activeNumber.transform.position.x, 4].gameObject.GetComponent<SpriteNum>().num;
                hitObject = numArray[(int)activeNumber.transform.position.x, 4];
                StartCoroutine(OnPlusNumber(activeNumber));
                break;
            }
        }
    }

    bool CheckCollision(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapPoint(position);
        if (hit != null)
        {
            hitObjectNum = hit.gameObject.GetComponent<SpriteNum>().num;
            hitObject = hit.gameObject;
        }
        return hit != null;
    }

    void ColorNextTo(GameObject NgameObject)
    {
        if(NgameObject == null)
        {
            numArray[(int)activeNumber.transform.position.x + 1, 0].GetComponent<SpriteRenderer>().color = new Color(86f / 255f, 168f / 255f, 198f / 255f);
            numArray[(int)activeNumber.transform.position.x - 1, 0].GetComponent<SpriteRenderer>().color = new Color(86f / 255f, 168f / 255f, 198f / 255f);
            return;
        }

        Vector3 plusx = new Vector3(1.0f, 0, 0);
        Vector3 plusy = new Vector3(0, 1.0f, 0);
        Vector2[] position = new Vector2[3];
        position[0] = NgameObject.transform.position + plusx;
        position[1] = NgameObject.transform.position - plusx;
        position[2] = NgameObject.transform.position - plusy;

        for (int i = 0; i < position.Length; i++)
        {
            if (position[i].x >= 0 && position[i].x < width && position[i].y >= 0 && position[i].y < height)
            {
                if (numArray[(int)position[i].x, (int)position[i].y] != null)
                {
                    numArray[(int)position[i].x, (int)position[i].y].GetComponent<SpriteRenderer>().color = new Color(86f / 255f, 168f / 255f, 198f / 255f);
                }
            }

        }
    }

    void CheckNextTo(GameObject NgameObject)
    {
        Vector3 plusx = new Vector3(1.0f, 0, 0);
        Vector3 plusy = new Vector3(0, 1.0f, 0);
        Vector2[] position = new Vector2[3];
        position[0] = NgameObject.transform.position + plusx;
        position[1] = NgameObject.transform.position - plusx;
        position[2] = NgameObject.transform.position - plusy;

        for (int i = 0; i < position.Length; i++)
        {
            Collider2D collider = Physics2D.OverlapPoint(position[i]);
            if (collider != null && NgameObject.GetComponent<SpriteNum>().num == collider.gameObject.GetComponent<SpriteNum>().num)
            {
                numArray[(int)collider.gameObject.transform.position.x, (int)collider.gameObject.transform.position.y] = null;
                numArray[(int)NgameObject.transform.position.x, (int)NgameObject.transform.position.y] = null;
                Destroy(collider.gameObject);
                Destroy(NgameObject);
            }
        }
    }

    void UpdateFallingBlocks()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y > 0; y--)
            {
                if (numArray[x, y] != null && numArray[x, y - 1] == null)
                {
                    GameObject fallingObj = numArray[x, y];
                    int newY = y - 1;

                    fallingObj.transform.position = new Vector3(x, newY, 0);
                    numArray[x, newY] = fallingObj;
                    numArray[x, y] = null;
                }
            }
        }
    }

    IEnumerator UpdateFallingBlocksCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            UpdateFallingBlocks();
        }
    }

    IEnumerator OnPlusNumber(GameObject onOBJ)
    {
        plusNum = onOBJ.GetComponent<SpriteNum>().num + hitObjectNum;
        if (plusNum >= 10)
        {
            plusNum -= 10;
        }
        Debug.Log(onOBJ.GetComponent<SpriteNum>().num + hitObjectNum + "=>" + plusNum);

        var plusNumber = Instantiate(Number[plusNum]);
        plusNumber.transform.position = hitObject.transform.position;
        int gridX = Mathf.RoundToInt(plusNumber.transform.position.x);
        int gridY = Mathf.RoundToInt(plusNumber.transform.position.y);
        numArray[gridX, gridY] = plusNumber;

        yield return null;
        Destroy(hitObject);
        Destroy(onOBJ);
        yield return new WaitForSeconds(0.5f);
        CheckNextTo(plusNumber);
        Destroy(nextNumber);
        activeNumber = null;
        nextNumber = null;
        CreateNowNumber(next);
    }

    void CheckOBJ()
    {
        bool isObj = false;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (numArray[i, j] != null)
                {
                    isObj = true;
                }
            }
        }

        if (!isObj)
        {
            GameClear();
        }
    }

    void GameClear()
    {
        Debug.Log("Game Clear");
        clearText.SetActive(true);
        menuButton.SetActive(true);
        float clearTime = countup;
        clearText.GetComponent<TextMeshProUGUI>().text = clearTime.ToString("f1");
        Time.timeScale = 0;
        conText.SetActive(true);
    }

    public void OnMenuButton()
    {
        SceneManager.LoadScene("MenuScene");
    }

}