<FONT size=20><B>BigCollections</B></FONT> - изначально задумывалась для одного из моих проектов, но потом стала использоваться во многих. Содержит несколько типов коллекций - List, BitList, Dictionary и прочие. Несмотря на название, именно коллекции с префиксом Big как раз пока что и не работают как надо. А для того, чтобы работали остальные коллекции, необходимо:
1. В "Обозревателе решений" выделить основной проект и нажать "Показать все файлы".
2. Открыть файл obj\Debug\net6.0\\&lt;ProjectName&gt;.GlobalUsings.g.cs.
3. Скопировать содержимое и вставить его в любой пользовательский файл (например, Program.cs), удалив строки global using global::System.Collections.Generic; и global using global::System.Linq;.
4. Если некоторые типы из этих пространств имен вам все же нужны (например, HashSet), добавьте в конец вставленного на предыдущем шаге блока global using G = global::System.Collections.Generic; и перед именами этих типов вставьте G..
5. Открыть файл &lt;ProjectName&gt;.csproj и удалить строку &lt;ImplicitUsings&gt;enable&lt;/ImplicitUsings&gt;.<BR>
Примечание: точность всех названий до бита не гарантируется. Но вы же человек, а не машина? На всякий случай проверяйте другие названия, похожие на вышеуказанные.<BR>
Классы похожи на соответствующие классы mscorlib, но в некоторых случаях более оптимизированы (а в некоторых - наоборот, это же The Fastest .NET Yet!). Оригинал взят с https://referencesource.microsoft.com/. В частности, оптимизированы для устранения лишних проходов по коллекциям методы List.Insert, List.InsertRange, SortedDictionary.Add, SortedDictionary.Search и прочие. Исправлен баг, вследствие которого InsertRange с IEnumerable, но не ICollection занимало квадратичное время.<BR>
Возможно, не всем понятно назначение класса Chain, так как у него нет аналогов в mscorlib. Он представляет собой цепь целых чисел, которая не хранится полностью (в отличие от Enumerable.Range), а только начало и длина. По ней можно даже итерироваться, не загружая в память полностью. Она создается полностью только при преобразовании в другой тип.<BR>
Отдельного внимания заслуживает файл OptimizedLinq.cs. Он делает именно то, что в нем написано. Его код похож на автогенерируемый и по факту на 80% таким является. 19% - код, скопированный с автогенерирумемого, в котором произведены некоторые замены, и лишь 1% написан вручную. Также в нем существенно больше методов и разных их реализаций, чем в классическом LINQ.<BR>
Назначение большинства методов понятно из названия. Названия сделаны более понятными непосвященному, чем в классическом LINQ, где они взяты из SQL, который точно мало кому понятен. Методы, которые мне не удалось реализовать, просто вызывают методы классического LINQ (но тоже иногда переименованы).<BR>
В отличие от классического LINQ, реализованные мной методы возвращают List, а не IEnumerable (кроме преобразующих в конкретный тип коллекции), вследствие чего следующий метод выполняется существенно быстрее, чем в классическом LINQ. А некоторые методы вообще выполняются с другой асимптотикой - например, Count() для строки.<BR>
Все методы, у которых есть функция конверсии, поставляются с двумя ее версиями - только от элементов и от элементов и номера. Даже вызывающие классический LINQ.<BR>
Вот описание методов, назначение которых может быть непонятно:<BR>
Any без параметров (только последовательность) - проверяет, что последовательность не пуста.<BR>
CopyDoubleList, CopyTripleList - копирует двумерный или трехмерный список полностью, а не только ссылки на (n - 1)-мерные списки, как new List<T>().<BR>
Fill - заполняет список одинаковыми значениями или функциями от номера элемента, позволяя не вычислять каждый раз пустой элемент. К сожалению, если вы укажете в elem объект ссылочного типа, список будет заполнен ссылками на один и тот же объект. Если кто знает, как исправить эту проблему - пишите в обсуждении. Для временного обхода используйте вторую функцию с _ вместо номера.<BR>
FindXIndexes, IndexesOfX - возвращает сразу ВСЕ индексы, в которых находится нужный элемент, а не только первый или последний.<BR>
Median - сортирует последовательность и возвращает значение, находящееся в середине.<BR>
Max/Mean/Median/Min(params <type>[] source) - позволяет вычислить нужную функцию, передав ей параметры один за другим, а не в виде коллекции - как Math.Max и Math.Min, только для произвольного числа параметров (но не меньше трех, так как для одного элемента функция тривиальна, а при двух функция существенно медленнее функции из Math). Рекомендуется указывать имя класса хотя бы одной буквой, так как иначе для двух элементов возникнет конфликт методов.<BR>
OfType - возвращает ТОЛЬКО элементы требуемого типа, игнорируя остальные. Этот метод работает с нетипизированным IEnumerable.<BR>
RepresentIntoNumbers - присваивает элементам последовательности номера, одинаковые для равных элементов и разные - для отличающихся, и возвращает список этих номеров. Нумерация начинается с нуля.<BR>
SetInnerType - приводит каждый элемент последовательности к нужному типу с помощью либо оператора явного приведения, либо пользовательской функции. Может выбросить InvalidCastException. Этот метод работает с нетипизированным IEnumerable.<BR>
ToArray поддерживает реализации как без параметров (прямой аналог метода классического LINQ), так и с параметрами - опять же для экономии количества проходов по последовательности. Еще больше самых разных реализаций доступно в методе ToDictionary.<BR>
TryGetCountEasily - пробует получить количество элементов без полного перебора коллекции. В отличие от метода классического LINQ, возвращает true для существенно большего множества типов. Этот метод работает как с типизированным, так и с нетипизированным IEnumerable.<BR>
Wrap и TryWrap - позволяют избежать как лишних действий, так и лишней строки кода в случае, когда результат вычисления очереднго LINQ-метода используется в следующем несколько раз. По сути ничего функционального не делают, а только кэшируют поданную им последовательность, позволяя написать x вместо громоздкого выражения, и вызывают внутреннюю функцию.<BR>
Тесты показали, что при первом запуске данные методы могут быть даже медленнее классических, становясь быстрее лишь после "разогрева" (и то только те, которые предназначены быть быстрыми).<BR>
Метод JoinIntoSingle необходимо вызывать, либо предварительно приведя средний тип двумерной коллекции к List, либо указывая в угловых скобках сначала этот средний тип вместе со внутренним, а затем целевой тип - если хоть один не указать, метод не работает!<BR>
Благодаря неоценимому участию Элд Хасп класс BitList способен копировать фрагменты своих экземпляров существенно быстрее, чем бит за битом. Сам я с этим бы не справился. <B>Элд Хасп</B>, еще раз благодарю вас, вы сделали очень много хорошего!<BR>
Также списки (в том числе BitList) поддерживают несколько методов, которых нет в классических реализациях:<BR>
GetRange, принимающий именно Range, а не index и count, а также индексатор такой же формы.<BR>
SetRange - записывает коллекцию элементов "поверх" того, что было в списке, начиная с index.<BR>
ReplaceRange - по форме удаляет диапазон и вставляет в место его бывшего начала нужную коллекцию, а по содержанию общие индексы копирует, а только отличающиеся удаляет или вставляет. Метод не проверял, могут быть ошибки.<BR>
Также возможны ошибки и в других не самых популярных классах и методах, так как протестировать всё и во всех случаях невозможно.<BR>
	<B>Обновление 1.</B> Добавилось огромное количество новых методов для работы с переменными типов Span<T> и ReadOnlySpan<T>, которые по неизвестным мне и не зависящим от меня причинам не наследуются от IEnumerable<T>.
Payeer: P19926501<BR>
Monero: 4AHvZX6BHNcZ6T2iCq4Ruu3nGXipEzjdyYPpvLGMqCzXartsMJoFBxRjXEeKRXDu96JCyYvvPunNnSMBeKYTS8iXBw9z6p3<BR>
