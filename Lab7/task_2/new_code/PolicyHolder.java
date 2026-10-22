package variant6_insurancepremiumcalculator;

import java.math.BigDecimal;
import java.util.Objects;

/** Тип страхового полісу. Використовується лише як ключ для пошуку стратегії: */
enum PolicyType {
    AUTO,
    HEALTH,
    PROPERTY,
    TRAVEL,
    /** Додано для демонстрації OCP  */
    LIFE
}


public final class PolicyHolder {
    private final String holderId;
    private final int age;
    private final BigDecimal insuredAmount;
    private final boolean hasPriorClaims;

    public PolicyHolder(String holderId, int age, BigDecimal insuredAmount, boolean hasPriorClaims) {
        this.holderId = holderId;
        this.age = age;
        this.insuredAmount = insuredAmount;
        this.hasPriorClaims = hasPriorClaims;
    }

    public String getHolderId() { return holderId; }
    public int getAge() { return age; }
    public BigDecimal getInsuredAmount() { return insuredAmount; }
    public boolean hasPriorClaims() { return hasPriorClaims; }
}
