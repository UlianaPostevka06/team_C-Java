package Task1_Bridge;

import common.Log;

/*
 * Усі стихії в одному файлі.
 * Це класи без public: вони видимі в межах пакета Task1_Bridge.
 * Кожна стихія лишається окремим класом, а нова додається ще одним класом
 * без змін в уміннях та інших стихіях.
 */

/** ConcreteImplementor: вогонь, ефект - підпал. */
class Fire implements Element {

    @Override
    public String name() {
        return "Fire";
    }

    @Override
    public void applyTo(Target target) {
        target.addEffect("Burning");
        Log.log("Fire", target.getName() + " is set on fire (burning for 3 turns)");
    }
}

/** ConcreteImplementor: крига, ефект - уповільнення. */
class Ice implements Element {

    @Override
    public String name() {
        return "Ice";
    }

    @Override
    public void applyTo(Target target) {
        target.addEffect("Slowed");
        Log.log("Ice", target.getName() + " is slowed (speed -30%)");
    }
}

/** ConcreteImplementor: блискавка, ефект - оглушення. */
class Lightning implements Element {

    @Override
    public String name() {
        return "Lightning";
    }

    @Override
    public void applyTo(Target target) {
        target.addEffect("Stunned");
        Log.log("Lightning", target.getName() + " is stunned (2 seconds)");
    }
}

/** ConcreteImplementor: отрута, нова стихія для демонстрації розширення. */
class Poison implements Element {

    @Override
    public String name() {
        return "Poison";
    }

    @Override
    public void applyTo(Target target) {
        target.addEffect("Poisoned");
        Log.log("Poison", target.getName() + " is poisoned (-4 health per turn)");
    }
}
