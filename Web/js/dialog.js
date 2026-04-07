document.addEventListener('DOMContentLoaded', () => {
    const questionsRoot = document.getElementById('questions');
    const questionInput = document.getElementById('question-input');
    const askButton = document.getElementById('question-button');
    const voiceButton = document.getElementById('question-voice');
    const voiceOutput = document.getElementById('voice-output');
    const historyWrapper = document.getElementById('history');
    const voiceStatus = document.getElementById('voice-status');
    const knowledgeCounter = document.getElementById('knowledge-counter');

    if (!questionsRoot || !questionInput || !askButton || !voiceButton || !voiceOutput || !historyWrapper || !voiceStatus) {
        return;
    }

    const knowledgeEntries = [
        { question: 'Что такое имитационное моделирование?', triad: ['Имитационное моделирование', 'является', 'методом исследования системы с помощью ее модели'] },
        { question: 'Для чего используют моделирование?', triad: ['Моделирование', 'используют', 'для исследования объектов, процессов и явлений'] },
        { question: 'Чем имитационное моделирование отличается от аналитического?', triad: ['Имитационное моделирование', 'отличается', 'от аналитического моделирования алгоритмическим воспроизведением структуры системы'] },
        { question: 'Когда применяют имитационное моделирование?', triad: ['Имитационное моделирование', 'применяют', 'когда аналитическое решение слишком сложно или недоступно'] },
        { question: 'Что заменяет модель при имитационном моделировании?', triad: ['Модель', 'заменяет', 'реальную систему при проведении экспериментов'] },
        { question: 'Что позволяет получить имитационная модель?', triad: ['Имитационная модель', 'позволяет получить', 'информацию о поведении исследуемой системы'] },
        { question: 'Когда моделирование выгоднее реального эксперимента?', triad: ['Моделирование', 'выгоднее', 'когда реальные эксперименты дороги, опасны или невозможны'] },
        { question: 'Что позволяет изучать моделирование во времени?', triad: ['Моделирование', 'позволяет изучать', 'поведение системы во времени'] },
        { question: 'Где применяют имитационное моделирование для обучения?', triad: ['Имитационное моделирование', 'используют', 'для создания тренажеров и симуляторов оборудования'] },
        { question: 'Что воспроизводится алгоритмически в имитационной модели?', triad: ['Структура системы', 'воспроизводится', 'алгоритмически в имитационной модели'] },

        { question: 'Когда систему моделируют из-за дороговизны экспериментов?', triad: ['Имитационное моделирование', 'применяют', 'если эксперименты на реальном объекте слишком дороги'] },
        { question: 'Когда систему моделируют из-за опасности экспериментов?', triad: ['Имитационное моделирование', 'применяют', 'если эксперименты на реальном объекте опасны'] },
        { question: 'Когда систему моделируют из-за невозможности эксперимента?', triad: ['Имитационное моделирование', 'применяют', 'если эксперимент на реальном объекте невозможен'] },
        { question: 'Когда моделируют еще не существующую систему?', triad: ['Имитационное моделирование', 'используют', 'на этапе проектирования еще не существующей системы'] },
        { question: 'Почему сложно построить аналитическую модель?', triad: ['Аналитическую модель', 'сложно построить', 'для сложных систем со случайными факторами и нелинейными связями'] },
        { question: 'Что позволяет сжатие времени в модели?', triad: ['Сжатие времени в модели', 'позволяет', 'быстро наблюдать длительные процессы'] },

        { question: 'Что такое дискретно-событийное моделирование?', triad: ['Дискретно-событийное моделирование', 'представляет', 'работу системы как последовательность событий'] },
        { question: 'Когда меняется состояние системы в дискретно-событийной модели?', triad: ['Состояние системы в дискретно-событийной модели', 'изменяется', 'только в моменты наступления событий'] },
        { question: 'Где применяют дискретно-событийное моделирование?', triad: ['Дискретно-событийное моделирование', 'применяют', 'в логистике, складах и системах массового обслуживания'] },
        { question: 'Что такое системная динамика?', triad: ['Системная динамика', 'является', 'подходом к моделированию сложных систем с обратными связями'] },
        { question: 'Кто предложил системную динамику?', triad: ['Системную динамику', 'предложил', 'Джей Форрестер'] },
        { question: 'Что рассматривает системная динамика?', triad: ['Системная динамика', 'рассматривает', 'процессы как непрерывные потоки'] },
        { question: 'Где применяют системную динамику?', triad: ['Системную динамику', 'применяют', 'в макроэкономике, экологии и стратегическом управлении'] },
        { question: 'Что такое агентное моделирование?', triad: ['Агентное моделирование', 'представляет', 'систему как совокупность взаимодействующих агентов'] },
        { question: 'Что имеет каждый агент в агентной модели?', triad: ['Каждый агент в агентной модели', 'имеет', 'собственное поведение, цели и правила взаимодействия'] },
        { question: 'Где применяют агентное моделирование?', triad: ['Агентное моделирование', 'применяют', 'для моделирования толпы, транспорта и рыночного поведения'] },

        { question: 'С чего начинается построение имитационной модели?', triad: ['Построение имитационной модели', 'начинается', 'с постановки проблемы и целей исследования'] },
        { question: 'Что делают на этапе концептуального моделирования?', triad: ['Концептуальное моделирование', 'включает', 'описание структуры системы и выделение ключевых связей'] },
        { question: 'Что такое формализация модели?', triad: ['Формализация модели', 'представляет', 'перевод концептуальной схемы в логико-математический вид'] },
        { question: 'Что включает программная реализация модели?', triad: ['Программная реализация модели', 'включает', 'создание компьютерной модели в среде разработки или на языке программирования'] },
        { question: 'Что проверяет верификация?', triad: ['Верификация', 'проверяет', 'правильность работы модели без программных ошибок'] },
        { question: 'Что проверяет валидация?', triad: ['Валидация', 'проверяет', 'адекватность модели реальной системе'] },
        { question: 'Для чего проводят эксперименты с моделью?', triad: ['Эксперименты с моделью', 'проводят', 'для анализа поведения системы при различных параметрах'] },
        { question: 'Что делают на этапе анализа результатов?', triad: ['Анализ результатов', 'включает', 'интерпретацию данных и принятие решений'] },

        { question: 'Какое преимущество дает моделирование по стоимости?', triad: ['Имитационное моделирование', 'обеспечивает', 'экономию средств на этапе проектирования'] },
        { question: 'Какое преимущество дает моделирование по безопасности?', triad: ['Имитационное моделирование', 'повышает', 'безопасность проверки аварийных сценариев'] },
        { question: 'Какое преимущество дает моделирование во времени?', triad: ['Имитационное моделирование', 'позволяет', 'ускорять или замедлять исследуемые процессы'] },
        { question: 'Какое преимущество дает визуализация модели?', triad: ['Визуализация модели', 'делает', 'проект понятнее для разработчика и заказчика'] },
        { question: 'Какой недостаток связан со стоимостью разработки модели?', triad: ['Сложная имитационная модель', 'требует', 'квалифицированных специалистов и затрат на разработку'] },
        { question: 'Какой риск связан с неверными исходными данными?', triad: ['Неверные исходные данные модели', 'приводят', 'к ложным результатам моделирования'] },
        { question: 'Почему валидация бывает сложной?', triad: ['Валидация уникальной модели', 'затрудняется', 'отсутствием эталона для сравнения'] },
        { question: 'Что позволяет изучать симулятор 3D-принтера?', triad: ['Симулятор 3D-принтера', 'позволяет изучать', 'кинематику, нагрев и экструзию без риска для оборудования'] },

        { question: 'Кто считается создателем современной 3D-печати?', triad: ['Создателем современной 3D-печати', 'считается', 'Чарльз Халл'] },
        { question: 'Что запатентовал Чарльз Халл?', triad: ['Чарльз Халл', 'запатентовал', 'технологию стереолитографии в 1984 году'] },
        { question: 'Кто изобрел FDM?', triad: ['Скотт Крамп', 'изобрел', 'технологию FDM в 1988 году'] },
        { question: 'Что лежит в основе большинства настольных 3D-принтеров?', triad: ['Технология FDM', 'лежит в основе', 'большинства доступных настольных 3D-принтеров'] },
        { question: 'Что такое проект RepRap?', triad: ['Проект RepRap', 'является', 'инициативой по созданию самовоспроизводимого открытого 3D-принтера'] },
        { question: 'Кто инициировал проект RepRap?', triad: ['Проект RepRap', 'инициировал', 'Адриан Боуер'] },
        { question: 'Что произошло в 2009 году с патентами на FDM?', triad: ['Истечение патентов на FDM в 2009 году', 'ускорило', 'рост рынка потребительских 3D-принтеров'] },
        { question: 'Что дало развитие ARM-плат для 3D-принтеров?', triad: ['Переход на 32-битные ARM-платы', 'позволил', 'выполнять более сложные расчеты кинематики в реальном времени'] },
        { question: 'Что сделали бесшумные драйверы шаговых двигателей?', triad: ['Бесшумные драйверы шаговых двигателей', 'снизили', 'шум работы 3D-принтеров'] },
        { question: 'Во что эволюционировали ранние конструкции принтеров?', triad: ['Конструкции 3D-принтеров', 'эволюционировали', 'от простых самодельных рам к жестким металлическим системам'] },

        { question: 'Что делает экструдер?', triad: ['Экструдер', 'захватывает', 'филамент и подает его к хотэнду'] },
        { question: 'Что делает хотэнд?', triad: ['Хотэнд', 'нагревает', 'филамент до вязкотекучего состояния перед выходом через сопло'] },
        { question: 'Для чего нужна нагреваемая платформа?', triad: ['Нагреваемая платформа', 'удерживает', 'модель и улучшает адгезию первых слоев'] },
        { question: 'Для чего нужны шаговые двигатели?', triad: ['Шаговые двигатели', 'обеспечивают', 'точное перемещение узлов по осям X, Y и Z'] },
        { question: 'Для чего нужна материнская плата принтера?', triad: ['Материнская плата принтера', 'управляет', 'двигателями, нагревом и логикой печати'] },
        { question: 'Для чего нужна рама 3D-принтера?', triad: ['Рама 3D-принтера', 'обеспечивает', 'жесткость конструкции и стабильность печати'] },
        { question: 'Что такое Bowden-система?', triad: ['Bowden-система', 'размещает', 'мотор экструдера на раме отдельно от печатающей головки'] },
        { question: 'Что такое Direct Drive?', triad: ['Direct Drive', 'располагает', 'экструдер непосредственно рядом с хотэндом'] },
        { question: 'Почему Direct Drive удобен для TPU?', triad: ['Direct Drive', 'подходит', 'для печати гибкими материалами вроде TPU'] },
        { question: 'Из чего состоит хотэнд?', triad: ['Хотэнд', 'состоит', 'из радиатора, термобарьера, нагревательного блока и сопла'] },
        { question: 'Почему латунь часто используют для сопел?', triad: ['Латунные сопла', 'обладают', 'хорошей теплопроводностью'] },
        { question: 'Для чего используют закаленные сопла?', triad: ['Закаленные сопла', 'используют', 'для печати абразивными материалами'] },
        { question: 'Что такое кинематика принтера?', triad: ['Кинематика принтера', 'определяет', 'схему перемещения стола и печатающей головки'] },
        { question: 'Почему жесткая рама важна для точности?', triad: ['Жесткая рама', 'снижает', 'вибрации и паразитные колебания при печати'] },
        { question: 'Как движется стол в декартовой схеме?', triad: ['Стол в декартовой схеме', 'движется', 'по оси Y'] },
        { question: 'Как движется стол в CoreXY?', triad: ['Стол в CoreXY', 'перемещается', 'только по оси Z'] },

        { question: 'Что такое Creality K1 SE?', triad: ['Creality K1 SE', 'является', 'высокоскоростным FFF 3D-принтером серии K1'] },
        { question: 'Какую технологию печати использует Creality K1 SE?', triad: ['Creality K1 SE', 'использует', 'технологию FFF с послойным наплавлением термопластика'] },
        { question: 'Какая кинематика у Creality K1 SE?', triad: ['Creality K1 SE', 'имеет', 'кинематику CoreXY'] },
        { question: 'Почему CoreXY подходит для быстрой печати?', triad: ['Кинематика CoreXY', 'позволяет', 'снижать массу подвижных узлов и повышать скорость печати'] },
        { question: 'Какой экструдер установлен в Creality K1 SE?', triad: ['Creality K1 SE', 'оснащается', 'Direct Drive экструдером'] },
        { question: 'Какое сопло установлено в Creality K1 SE?', triad: ['Creality K1 SE', 'использует', 'сопло со стальным наконечником и интегрированным термобарьером'] },
        { question: 'До какой температуры нагревается сопло Creality K1 SE?', triad: ['Сопло Creality K1 SE', 'нагревается', 'до 300 градусов Цельсия'] },
        { question: 'До какой температуры нагревается стол Creality K1 SE?', triad: ['Нагреваемый стол Creality K1 SE', 'нагревается', 'до 100 градусов Цельсия'] },
        { question: 'Какие материалы поддерживает Creality K1 SE?', triad: ['Creality K1 SE', 'поддерживает', 'Hyper PLA, PLA, PETG и TPU'] },
        { question: 'Какая максимальная скорость печати у Creality K1 SE?', triad: ['Creality K1 SE', 'развивает', 'скорость печати до 600 мм/с'] },
        { question: 'Какое ускорение поддерживает Creality K1 SE?', triad: ['Creality K1 SE', 'поддерживает', 'ускорение до 20000 мм/с2'] },
        { question: 'Какой объем печати у Creality K1 SE?', triad: ['Creality K1 SE', 'обладает', 'областью печати 220 x 220 x 250 мм'] },
        { question: 'Какой диаметр сопла у Creality K1 SE?', triad: ['Диаметр штатного сопла Creality K1 SE', 'составляет', '0.4 мм'] },
        { question: 'Какой диаметр филамента использует K1 SE?', triad: ['Creality K1 SE', 'использует', 'филамент диаметром 1.75 мм'] },
        { question: 'Какой диапазон высоты слоя поддерживает K1 SE?', triad: ['Creality K1 SE', 'поддерживает', 'высоту слоя от 0.1 до 0.35 мм'] },
        { question: 'Как передаются файлы на Creality K1 SE?', triad: ['Файлы на Creality K1 SE', 'передаются', 'через USB и WiFi'] },
        { question: 'Какой облачный сервис поддерживает Creality K1 SE?', triad: ['Creality K1 SE', 'поддерживает', 'интеграцию с Creality Cloud'] },
        { question: 'Поддерживает ли K1 SE обновления по воздуху?', triad: ['Creality K1 SE', 'поддерживает', 'OTA-обновления прошивки'] },
        { question: 'Какой слайсер используют для Creality K1 SE?', triad: ['Для Creality K1 SE', 'используют', 'слайсер Creality Print'] },
        { question: 'Какой формат файлов печатает K1 SE?', triad: ['Creality K1 SE', 'печатает', 'G-code файлы'] },
        { question: 'Какой объем встроенного хранилища у K1 SE?', triad: ['Creality K1 SE', 'имеет', 'встроенное хранилище объемом 8 ГБ'] },
        { question: 'Подходит ли Creality K1 SE новичкам?', triad: ['Creality K1 SE', 'подходит', 'для начинающих благодаря автонастройке и простой подготовке к печати'] },
        { question: 'Чем K1 SE отличается от закрытых моделей серии K1?', triad: ['Creality K1 SE', 'отличается', 'от закрытых моделей серии K1 открытой конструкцией без боковых панелей и дверцы'] },
        { question: 'Как работает автокалибровка K1 SE?', triad: ['Автокалибровка K1 SE', 'выполняет', 'автоматическое выравнивание и подготовку принтера к печати'] },
        { question: 'Что делает Input Shaping в K1 SE?', triad: ['Input Shaping', 'снижает', 'вибрации и артефакты поверхности при высоких скоростях'] },
        { question: 'Зачем нужен G-сенсор в K1 SE?', triad: ['G-сенсор', 'измеряет', 'резонансы по осям для компенсации вибраций'] },
        { question: 'Зачем нужна жесткая рама в K1 SE?', triad: ['Жесткая рама K1 SE', 'повышает', 'стабильность позиционирования и качество печати'] },
        { question: 'Для чего нужен Direct Drive в K1 SE?', triad: ['Direct Drive экструдер K1 SE', 'помогает', 'точно подавать филамент и печатать гибкими материалами'] },
        { question: 'Для чего нужно покрытие PEI?', triad: ['Покрытие PEI', 'обеспечивает', 'надежное прилипание первого слоя и удобное снятие модели'] },
        { question: 'Какая платформа у K1 SE?', triad: ['Платформа K1 SE', 'имеет', 'гибкую пружинную стальную пластину с покрытием PEI'] },

        { question: 'Что показывает принцип работы на видео?', triad: ['Видеоролик на странице принципа работы', 'показывает', 'процесс высокоскоростной 3D-печати'] },
        { question: 'Где применяют 3D-печать в медицине?', triad: ['3D-печать в медицине', 'используют', 'для протезов, анатомических моделей и подготовки к операциям'] },
        { question: 'Где применяют 3D-печать в машиностроении?', triad: ['3D-печать в машиностроении', 'используют', 'для изготовления сложных и облегченных деталей'] },
        { question: 'Где применяют 3D-печать в архитектуре?', triad: ['3D-печать в архитектуре', 'используют', 'для макетов зданий и строительных конструкций'] },
        { question: 'Где применяют 3D-печать в образовании и дизайне?', triad: ['3D-печать в образовании и дизайне', 'используют', 'для учебных пособий, прототипирования и художественных объектов'] },
        { question: 'Где применяют 3D-печать для ремонта?', triad: ['3D-печать для ремонта', 'используют', 'для восстановления редких деталей и запчастей'] },
        { question: 'Что позволяет 3D-печать при импортозамещении?', triad: ['3D-печать при импортозамещении', 'позволяет', 'быстро изготавливать необходимые детали локально'] },
        { question: 'Почему 3D-печать удобна для прототипирования?', triad: ['3D-печать', 'ускоряет', 'переход от цифровой модели к физическому прототипу'] },

        { question: 'Как выглядит Creality K1 SE?', triad: ['Creality K1 SE', 'выглядит', 'так <img class="kb-clickable-media" src="assets/printer_full.png" alt="Creality K1 SE">'] },
        { question: 'Где посмотреть работу Creality K1 SE?', triad: ['Работа Creality K1 SE', 'показана', 'в видеоролике <video controls preload="metadata"><source src="assets/RenderVideo.mp4" type="video/mp4">Ваш браузер не поддерживает воспроизведение видео.</video>'] }
    ];

    const answerKnowledge = knowledgeEntries.map((entry) => entry.question);
    const knowledge = knowledgeEntries.map((entry) => entry.triad);

    const endings = [
        ['ет', '(ет|ут|ют)'],
        ['ут', '(ет|ут|ют)'],
        ['ют', '(ет|ут|ют)'],
        ['ит', '(ит|ат|ят)'],
        ['ат', '(ит|ат|ят)'],
        ['ят', '(ит|ат|ят)'],
        ['ется', '(ет|ут|ют)ся'],
        ['утся', '(ет|ут|ют)ся'],
        ['ются', '(ет|ут|ют)ся'],
        ['ится', '(ит|ат|ят)ся'],
        ['атся', '(ит|ат|ят)ся'],
        ['ятся', '(ит|ат|ят)ся'],
        ['ается', '(ает|ают)ся'],
        ['ают', '(ает|ают)'],
        ['ает', '(ает|ают)'],
        ['уется', '(ует|уют)ся'],
        ['уют', '(ует|уют)'],
        ['ует', '(ует|уют)'],
        ['ен', 'ен'],
        ['ана', 'ана'],
        ['ано', 'ано'],
        ['ены', 'ены'],
        ['на', 'на'],
        ['но', 'но'],
        ['такое', '- это']
    ];

    const blacklist = ['замена', 'замены', 'атрибут', 'маршрут', 'член', 'нет'];
    const separatorPattern = /[\'",.!?()[\]\\/:\-;]/g;
    const mediaPattern = /<img|<video|<audio/i;
    const directQuestionMap = new Map(
        answerKnowledge.map((question, index) => [normalize(question), buildAnswer(knowledge[index])])
    );

    let recognition = null;
    let isListening = false;
    let activeAudio = null;
    let stopRequested = false;

    if (knowledgeCounter) {
        knowledgeCounter.textContent = `Доступно вопросов: ${answerKnowledge.length}. Триад в базе: ${knowledge.length}.`;
    }

    renderQuestions();
    initSpeechRecognition();

    askButton.addEventListener('click', () => ask());
    voiceButton.addEventListener('click', toggleVoiceInput);
    questionInput.addEventListener('keydown', (event) => {
        if (event.key === 'Enter') {
            event.preventDefault();
            ask();
        }
    });

    function renderQuestions() {
        questionsRoot.innerHTML = '';

        answerKnowledge.forEach((question, index) => {
            const answerPreview = stripHtml(buildAnswer(knowledge[index])).slice(0, 120);
            const card = document.createElement('button');
            card.type = 'button';
            card.className = 'knowledge-element';
            card.innerHTML = `
                <span class="knowledge-question">${question}</span>
                <span class="knowledge-answer">${answerPreview}${answerPreview.length >= 120 ? '...' : ''}</span>
            `;

            card.addEventListener('click', () => {
                questionInput.value = question;
                ask(question);
            });

            questionsRoot.appendChild(card);
        });
    }

    function initSpeechRecognition() {
        const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;

        if (!SpeechRecognition) {
            voiceButton.disabled = true;
            setVoiceStatus('Голосовой ввод не поддерживается браузером');
            return;
        }

        recognition = new SpeechRecognition();
        recognition.lang = 'ru-RU';
        recognition.interimResults = false;
        recognition.maxAlternatives = 1;

        recognition.onstart = () => {
            isListening = true;
            voiceButton.textContent = 'Слушаю...';
            setVoiceStatus('Голосовой ввод: идет распознавание');
        };

        recognition.onresult = (event) => {
            const transcript = event.results[0][0].transcript.trim();
            questionInput.value = transcript.endsWith('?') ? transcript : `${transcript}?`;
            ask(questionInput.value);
        };

        recognition.onerror = (event) => {
            isListening = false;
            voiceButton.textContent = 'Голосовой ввод';

            if (event.error === 'not-allowed' || event.error === 'service-not-allowed') {
                setVoiceStatus('Нет доступа к микрофону');
                return;
            }

            if (event.error === 'no-speech') {
                setVoiceStatus('Речь не распознана, попробуйте еще раз');
                return;
            }

            setVoiceStatus(`Ошибка распознавания: ${event.error}`);
        };

        recognition.onend = () => {
            isListening = false;
            voiceButton.textContent = 'Голосовой ввод';

            if (!voiceStatus.textContent.startsWith('Ошибка') && !voiceStatus.textContent.includes('Нет доступа')) {
                setVoiceStatus('Голосовой ввод: готов');
            }
        };
    }

    function toggleVoiceInput() {
        if (!recognition) {
            return;
        }

        if (isListening) {
            recognition.stop();
            return;
        }

        recognition.start();
    }

    function ask(customQuestion) {
        const question = (customQuestion || questionInput.value).trim();

        if (!question) {
            return;
        }

        stopAudioPlayback();
        appendMessage(question, 'question__wrapper');

        const answer = getAnswer(question);
        const answerElement = appendMessage(answer, 'answer__wrapper', true);

        bindMediaPreview(answerElement);
        bindMediaLoadScroll(answerElement);
        scrollChatToBottom();

        if (voiceOutput.checked) {
            speakAnswer(stripHtml(answer));
        }

        questionInput.value = '';
    }

    function appendMessage(content, className, isHtml = false) {
        const message = document.createElement('div');
        message.className = `dialog-message ${className}`;

        const bubble = document.createElement('div');
        bubble.className = 'dialog-message__bubble';

        if (isHtml) {
            bubble.innerHTML = content;
        } else {
            bubble.textContent = content;
        }

        message.appendChild(bubble);
        historyWrapper.appendChild(message);
        return message;
    }

    function bindMediaPreview(container) {
        const image = container.querySelector('.kb-clickable-media');

        if (!image) {
            return;
        }

        image.addEventListener('click', () => {
            const overlay = document.createElement('div');
            overlay.className = 'image-wrapper';
            overlay.innerHTML = `
                <div class="image__container image__container--knowledge">
                    <button class="close-btn" type="button" aria-label="Закрыть просмотр">x</button>
                    <img src="${image.src}" alt="${image.alt}">
                </div>
            `;

            document.body.appendChild(overlay);

            const close = () => overlay.remove();
            overlay.querySelector('.close-btn').addEventListener('click', close);
            overlay.addEventListener('click', (event) => {
                if (event.target === overlay) {
                    close();
                }
            });
        });
    }

    function bindMediaLoadScroll(container) {
        container.querySelectorAll('img, video').forEach((media) => {
            media.addEventListener('load', scrollChatToBottom);
            media.addEventListener('loadedmetadata', scrollChatToBottom);
        });
    }

    function speakAnswer(text) {
        const cleanText = text.trim();
        if (!cleanText) {
            return;
        }

        stopRequested = false;
        playTtsQueue(splitText(cleanText, 180), cleanText, 0);
    }

    function playTtsQueue(chunks, fullText, index) {
        if (stopRequested || index >= chunks.length) {
            return;
        }

        const audio = new Audio(buildTtsUrl(chunks[index]));
        activeAudio = audio;
        audio.autoplay = true;

        audio.addEventListener('ended', () => {
            if (!stopRequested) {
                playTtsQueue(chunks, fullText, index + 1);
            }
        });

        audio.addEventListener('error', () => {
            fallbackSpeechSynthesis(fullText);
        });

        audio.play().catch(() => {
            fallbackSpeechSynthesis(fullText);
        });
    }

    function fallbackSpeechSynthesis(text) {
        stopAudioPlayback();

        if (!('speechSynthesis' in window)) {
            setVoiceStatus('Не удалось воспроизвести TTS');
            return;
        }

        window.speechSynthesis.cancel();

        const utterance = new SpeechSynthesisUtterance(text);
        utterance.lang = 'ru-RU';
        utterance.rate = 1;
        utterance.pitch = 1;

        const russianVoice = window.speechSynthesis.getVoices().find((voice) => voice.lang.startsWith('ru'));
        if (russianVoice) {
            utterance.voice = russianVoice;
        }

        window.speechSynthesis.speak(utterance);
    }

    function stopAudioPlayback() {
        stopRequested = true;

        if (activeAudio) {
            activeAudio.pause();
            activeAudio.src = '';
            activeAudio = null;
        }

        if ('speechSynthesis' in window) {
            window.speechSynthesis.cancel();
        }
    }

    function buildTtsUrl(text) {
        return `https://translate.googleapis.com/translate_tts?ie=UTF-8&client=tw-ob&tl=ru&q=${encodeURIComponent(text)}`;
    }

    function splitText(text, maxLength) {
        const words = text.split(/\s+/);
        const parts = [];
        let currentPart = '';

        words.forEach((word) => {
            const candidate = currentPart ? `${currentPart} ${word}` : word;
            if (candidate.length > maxLength && currentPart) {
                parts.push(currentPart);
                currentPart = word;
            } else {
                currentPart = candidate;
            }
        });

        if (currentPart) {
            parts.push(currentPart);
        }

        return parts;
    }

    function getAnswer(question) {
        const normalizedQuestion = normalize(question);

        if (directQuestionMap.has(normalizedQuestion)) {
            return directQuestionMap.get(normalizedQuestion);
        }

        const directIndex = answerKnowledge.findIndex((item) => normalizedQuestion.includes(normalize(item)));
        if (directIndex !== -1) {
            return buildAnswer(knowledge[directIndex]);
        }

        const text = small(question).replace(separatorPattern, ' ');
        const words = text.split(/\s+/).filter(Boolean).map(small);

        let result = false;
        let answer = '';

        for (let i = 0; i < words.length; i += 1) {
            const ending = getEnding(words[i]);

            if (ending < 0) {
                continue;
            }

            const wordBase = escapeRegExp(words[i].substring(0, words[i].length - endings[ending][0].length));
            let predicate = new RegExp(`.*${wordBase}${endings[ending][1]}.*`);

            if (endings[ending][0] === endings[ending][1] && words[i + 1]) {
                predicate = new RegExp(`.*${wordBase}${endings[ending][1]} ${escapeRegExp(words[i + 1])}.*`);
                i += 1;
            }

            const subjectReg = words.slice(i + 1).map(escapeRegExp).join('.*');
            if (subjectReg.length <= 2) {
                continue;
            }

            const subject = new RegExp(`.*${subjectReg}.*`);

            for (let j = 0; j < knowledge.length; j += 1) {
                const item = knowledge[j];
                if (
                    predicate.test(item[1].toLowerCase()) &&
                    (subject.test(item[0].toLowerCase()) || subject.test(stripHtml(item[2]).toLowerCase()))
                ) {
                    answer = buildAnswer(item);
                    result = true;
                    break;
                }
            }

            if (result) {
                break;
            }
        }

        if (!result) {
            const fallback = findBestMatch(question);
            if (fallback) {
                answer = buildAnswer(fallback);
                result = true;
            }
        }

        return result ? answer : 'Ответ не найден. Попробуйте спросить о моделировании, узлах 3D-принтера, характеристиках Creality K1 SE или областях применения.';
    }

    function findBestMatch(question) {
        const tokens = normalize(question).split(' ').filter((token) => token.length > 2);
        let bestMatch = null;
        let bestScore = 0;

        knowledge.forEach((item) => {
            const haystack = normalize(`${item[0]} ${item[1]} ${stripHtml(item[2])}`);
            let score = 0;

            tokens.forEach((token) => {
                if (haystack.includes(token)) {
                    score += 1;
                }
            });

            if (score > bestScore) {
                bestScore = score;
                bestMatch = item;
            }
        });

        return bestScore >= 2 ? bestMatch : null;
    }

    function buildAnswer(item) {
        return `${big(item[0])} ${item[1]} ${item[2]}`;
    }

    function getEnding(word) {
        if (blacklist.includes(word)) {
            return -1;
        }

        for (let i = 0; i < endings.length; i += 1) {
            if (word.endsWith(endings[i][0])) {
                return i;
            }
        }

        return -1;
    }

    function scrollChatToBottom() {
        requestAnimationFrame(() => {
            historyWrapper.scrollTop = historyWrapper.scrollHeight;
        });
    }

    function normalize(text) {
        return text
            .toLowerCase()
            .replace(/ё/g, 'е')
            .replace(separatorPattern, ' ')
            .replace(/\s+/g, ' ')
            .trim();
    }

    function stripHtml(text) {
        const temp = document.createElement('div');
        temp.innerHTML = text;
        return (temp.textContent || '').trim();
    }

    function escapeRegExp(text) {
        return text.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    }

    function big(text) {
        return text.charAt(0).toUpperCase() + text.slice(1);
    }

    function small(text) {
        return text.charAt(0).toLowerCase() + text.slice(1);
    }

    function setVoiceStatus(text) {
        voiceStatus.textContent = text;
    }

    window.speechSynthesis?.addEventListener?.('voiceschanged', () => {
        if (voiceOutput.checked) {
            window.speechSynthesis.getVoices();
        }
    });
});
