package Task1_Bridge;

import common.Log;

/*
 * Усі види умінь в одному файлі.
 * Це класи без public: вони видимі в межах пакета Task1_Bridge.
 * Кожне уміння лишається окремим класом і приймає будь-яку стихію через конструктор.
 */

/** RefinedAbstraction: удар по одній цілі. */
class SingleTargetStrike extends Skill {

    private static final int DAMAGE = 30;

    public SingleTargetStrike(Element element) {
        super(element);
    }

    @Override
    protected String skillName() {
        return "Single-Target Strike";
    }

    @Override
    public void use(String caster, Target... targets) {
        Target target = targets[0];
        target.takeDamage(DAMAGE);
        Log.log(caster, "uses " + title() + " on " + target.getName()
                + " (" + DAMAGE + " damage, health left " + target.getHealth() + ")");
        element.applyTo(target);
    }
}

/** RefinedAbstraction: удар по площі, зачіпає всі цілі. */
class AreaStrike extends Skill {

    private static final int DAMAGE = 15;

    public AreaStrike(Element element) {
        super(element);
    }

    @Override
    protected String skillName() {
        return "Area Strike";
    }

    @Override
    public void use(String caster, Target... targets) {
        Log.log(caster, "uses " + title() + " on " + targets.length + " targets");
        for (Target target : targets) {
            target.takeDamage(DAMAGE);
            Log.log(title(), target.getName() + " takes " + DAMAGE
                    + " damage (health left " + target.getHealth() + ")");
            element.applyTo(target);
        }
    }
}

/** RefinedAbstraction: захисний бар'єр, стихія діє на тих, хто його атакує. */
class ProtectiveBarrier extends Skill {

    public ProtectiveBarrier(Element element) {
        super(element);
    }

    @Override
    protected String skillName() {
        return "Barrier";
    }

    @Override
    public void use(String caster, Target... targets) {
        Log.log(caster, "raises " + title() + " around " + targets[0].getName());
    }

    /** Нападник, що вдарив по бар'єру, отримує ефект стихії. */
    public void onAttackerHit(Target attacker) {
        Log.log(title(), attacker.getName() + " hits the barrier");
        element.applyTo(attacker);
    }
}
