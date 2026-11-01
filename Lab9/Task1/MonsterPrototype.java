package task1_prototype;

/** Інтерфейс Prototype: об'єкт уміє створювати власну незалежну копію. */
public interface MonsterPrototype {

    /** Повертає глибоку копію об'єкта без повторного "дорогого" завантаження. */
    Monster copy();
}
