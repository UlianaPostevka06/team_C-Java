package Task3_Decorator;

/*
 * Усі п'ять ефектів в один файл.
 * Це класи без public: вони видимі в межах пакета Task3_Decorator.
 * Кожен ефект лишається окремим класом-обгорткою і лише задає свої модифікатори,
 * а новий ефект додається ще одним таким класом без змін в існуючих.
 */

/** ConcreteDecorator: зілля сили (збільшує атаку). */
class StrengthPotion extends EffectDecorator {

    public StrengthPotion(ICharacter wrapped) {
        super(wrapped, "Strength Potion");
    }

    @Override
    protected int attackModifier() {
        return 10;
    }
}

/** ConcreteDecorator: благословення (трохи збільшує атаку і захист). */
class Blessing extends EffectDecorator {

    public Blessing(ICharacter wrapped) {
        super(wrapped, "Blessing");
    }

    @Override
    protected int attackModifier() {
        return 2;
    }

    @Override
    protected int defenseModifier() {
        return 5;
    }
}

/** ConcreteDecorator: отрута (зменшує атаку і захист). */
class Poison extends EffectDecorator {

    public Poison(ICharacter wrapped) {
        super(wrapped, "Poison");
    }

    @Override
    protected int attackModifier() {
        return -4;
    }

    @Override
    protected int defenseModifier() {
        return -2;
    }
}

/** ConcreteDecorator: прокляття повільності (зменшує швидкість). */
class SlownessCurse extends EffectDecorator {

    public SlownessCurse(ICharacter wrapped) {
        super(wrapped, "Slowness Curse");
    }

    @Override
    protected int speedModifier() {
        return -5;
    }
}

/** ConcreteDecorator: щит (збільшує захист). */
class Shield extends EffectDecorator {

    public Shield(ICharacter wrapped) {
        super(wrapped, "Shield");
    }

    @Override
    protected int defenseModifier() {
        return 8;
    }
}
