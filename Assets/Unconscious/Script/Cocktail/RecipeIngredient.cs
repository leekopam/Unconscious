// 레시피에 필요한 재료와 그 양을 나타내는 클래스
[System.Serializable] // Unity Inspector에서 표시 가능하게 함
public class RecipeIngredient
{
    public Ingredient ingredient;   // 재료 객체
    public int amount;              // 해당 재료의 필요한 양

    // 생성자: 재료와 그 양을 초기화
    public RecipeIngredient(Ingredient ingredient, int amount)
    {
        this.ingredient = ingredient;
        this.amount = amount;
    }
}