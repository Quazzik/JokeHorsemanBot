public class GameRandom
{
    private float[] probabilities; // массив вероятностей выпадения элементов
    private int totalElements; // общее количество элементов
    private int currentPass; // текущий проход

    public GameRandom(int numberOfElements)
    {
        totalElements = numberOfElements;
        probabilities = new float[numberOfElements];
        currentPass = 0;

        // Инициализация вероятностей выпадения элементов в первом проходе
        for (int i = 0; i < probabilities.Length; i++)
        {
            probabilities[i] = 100 / probabilities.Length;
        }
    }

    public int GetNext()
    {
        // Генерация случайного числа от 0 до 99
        Random random = new Random();
        int randomNumber = random.Next(100);

        // Выбор элемента на основе вероятностей
        int selectedElement = 0;
        double cumulativeProbability = 0;
        for (int i = 0; i < totalElements; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomNumber < cumulativeProbability)
            {
                selectedElement = i;
                break;
            }
        }

        // Обновление вероятностей для следующего прохода
        UpdateProbabilities(selectedElement);

        // Увеличение текущего прохода
        currentPass++;

        return ++selectedElement;
    }

    private void UpdateProbabilities(int selectedElement)
    {
        // Уменьшение вероятности выбранного элемента в 10 раз
        probabilities[selectedElement] -= (probabilities[selectedElement] * 90) / 100;

        // Распределение вероятности между другими элементами
        for (int i = 0; i < totalElements; i++)
        {
            if (i != selectedElement)
            {
                probabilities[i] += (probabilities[i] * 30) / 100;
            }
        }

        // Проверка, что было проведено в полтора раза больше проходов чем всего элементов
        if (currentPass >= 1.5 * totalElements)
        {
            ResetProbabilities();
            currentPass = 0;
        }
    }

    private void ResetProbabilities()
    {
        // Восстановление вероятностей в первоначальное состояние
        for (int i = 0; i < totalElements; i++)
        {
            probabilities[i] = 100 / totalElements;
        }
    }
}
