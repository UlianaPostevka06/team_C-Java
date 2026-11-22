package Task1_Memento;

import common.Log;

import java.util.Map;
import java.util.TreeMap;

/** Caretaker: тримає слоти збережень. Вміст знімків не знає і змінити не може. */
public class SaveManager {

    private final Map<Integer, Memento> slots = new TreeMap<>();

    public void save(int slot, Hero hero) {
        slots.put(slot, hero.save());
        Log.log("SaveManager", "saved to slot " + slot + " (used slots: " + slots.keySet() + ")");
    }

    public void load(int slot, Hero hero) {
        Memento memento = slots.get(slot);
        if (memento == null) {
            Log.log("SaveManager", "slot " + slot + " is empty");
            return;
        }
        Log.log("SaveManager", "loading slot " + slot);
        hero.restore(memento);
    }
}
