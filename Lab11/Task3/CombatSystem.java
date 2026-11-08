package Task3_Decorator;

import common.Log;

/**
 * Client: бойова система. Працює лише з ICharacter,
 * тому персонаж з ефектами використовується так само, як звичайний.
 */
public class CombatSystem {

    public void attack(ICharacter attacker, ICharacter target) {
        int attack = attacker.getAttack();
        int defense = target.getDefense();
        int damage = Math.max(1, attack - defense);
        target.takeDamage(damage);
        Log.log("CombatSystem", attacker.getName() + " attacks " + target.getName()
                + ": attack " + attack + " - defense " + defense + " = damage " + damage
                + ", target hp " + target.getHealth());
    }
}
