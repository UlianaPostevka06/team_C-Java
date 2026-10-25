package variant6_insurancepremiumcalculator;

import java.math.BigDecimal;
import java.math.RoundingMode;

// Опис предметної області: Страхова компанія та актуарні розрахунки.
// Завдання коду: Обчислення страхового внеску (премії) залежно від типу страхового полісу та ризиків клієнта.
//
//

enum PolicyType {
    AUTO,
    HEALTH,
    PROPERTY,
    TRAVEL
}

class PolicyHolder {
    private String holderId;
    private int age;
    private BigDecimal insuredAmount;
    private boolean hasPriorClaims;

    public PolicyHolder(String holderId, int age, BigDecimal insuredAmount, boolean hasPriorClaims) {
        this.holderId = holderId;
        this.age = age;
        this.insuredAmount = insuredAmount;
        this.hasPriorClaims = hasPriorClaims;
    }

    public String getHolderId() { return holderId; }
    public int getAge() { return age; }
    public BigDecimal getInsuredAmount() { return insuredAmount; }
    public boolean isHasPriorClaims() { return hasPriorClaims; }
}

public class InsurancePremiumCalculator {

    public BigDecimal calculatePremium(PolicyHolder holder, PolicyType type) {
        BigDecimal basePremium = BigDecimal.ZERO;
        BigDecimal insuredAmount = holder.getInsuredAmount();

        if (type == PolicyType.AUTO) {
            // Автострахування: 3% від суми + 20% надбавка якщо водію менше 25 років або були страхові випадки
            basePremium = insuredAmount.multiply(new BigDecimal("0.03"));
            if (holder.getAge() < 25 || holder.isHasPriorClaims()) {
                basePremium = basePremium.multiply(new BigDecimal("1.20"));
            }
        } else if (type == PolicyType.HEALTH) {
            // Медичне страхування: $200 базово + $10 за кожен рік після 40 років
            basePremium = new BigDecimal("200.00");
            if (holder.getAge() > 40) {
                BigDecimal extraYears = new BigDecimal(holder.getAge() - 40);
                basePremium = basePremium.add(extraYears.multiply(new BigDecimal("10.00")));
            }
            if (holder.isHasPriorClaims()) {
                basePremium = basePremium.add(new BigDecimal("150.00"));
            }
        } else if (type == PolicyType.PROPERTY) {
            // Страхування майна: 0.5% від оціночної вартості нерухомості
            basePremium = insuredAmount.multiply(new BigDecimal("0.005"));
        } else if (type == PolicyType.TRAVEL) {
            // Туристичне страхування: фіксований поліс $30 + 50% надбавка для осіб віком 65+
            basePremium = new BigDecimal("30.00");
            if (holder.getAge() >= 65) {
                basePremium = basePremium.multiply(new BigDecimal("1.50"));
            }
        } else {
            throw new IllegalArgumentException("Тип страхування " + type + " не зареєстровано у системі.");
        }

        basePremium = basePremium.setScale(2, RoundingMode.HALF_UP);
        System.out.println("[INSURANCE] Клієнт " + holder.getHolderId() + " (Вік: " + holder.getAge() + "): Страховий поліс " + type + " -> Річна премія: $" + basePremium);
        return basePremium;
    }
    public static void main(String[] args) {
        InsurancePremiumCalculator calculator = new InsurancePremiumCalculator();

        PolicyHolder holder = new PolicyHolder(
                "C001",
                30,
                new BigDecimal("10000.00"),
                false
        );

        BigDecimal premium = calculator.calculatePremium(holder, PolicyType.AUTO);

        System.out.println("Результат розрахунку: $" + premium);
    }
}
