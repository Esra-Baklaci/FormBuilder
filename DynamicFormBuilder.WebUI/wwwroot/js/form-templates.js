window.FORM_TEMPLATES = {
    contact: {
        name: 'İletişim Formu',
        icon: 'bi-envelope-heart',
        color: '#6b8cff',
        fields: [
            { fieldType: 19, label: 'Bize Ulaşın', width: '100%', sortOrder: 0 },
            { fieldType: 20, label: 'Sorularınız için formu doldurun, en kısa sürede dönüş yapacağız.', width: '100%', sortOrder: 1 },
            { fieldType: 1, label: 'Ad Soyad', placeholder: 'Adınızı girin', isRequired: true, width: '50%', sortOrder: 2 },
            { fieldType: 4, label: 'E-posta', placeholder: 'ornek@mail.com', isRequired: true, width: '50%', sortOrder: 3 },
            { fieldType: 5, label: 'Telefon', placeholder: '05xx xxx xx xx', width: '50%', sortOrder: 4 },
            { fieldType: 10, label: 'Konu', options: 'Genel\nDestek\nSatış\nDiğer', isRequired: true, width: '50%', sortOrder: 5 },
            { fieldType: 2, label: 'Mesajınız', placeholder: 'Mesajınızı yazın...', isRequired: true, width: '100%', sortOrder: 6 }
        ]
    },
    job: {
        name: 'İş Başvurusu',
        icon: 'bi-briefcase',
        color: '#8b5cf6',
        fields: [
            { fieldType: 19, label: 'İş Başvuru Formu', width: '100%', sortOrder: 0 },
            { fieldType: 1, label: 'Ad Soyad', isRequired: true, width: '50%', sortOrder: 1 },
            { fieldType: 4, label: 'E-posta', isRequired: true, width: '50%', sortOrder: 2 },
            { fieldType: 5, label: 'Telefon', isRequired: true, width: '50%', sortOrder: 3 },
            { fieldType: 10, label: 'Pozisyon', options: 'Yazılım Geliştirici\nTasarımcı\nProje Yöneticisi\nStajyer', isRequired: true, width: '50%', sortOrder: 4 },
            { fieldType: 14, label: 'CV Yükle', isRequired: true, width: '100%', sortOrder: 5 },
            { fieldType: 2, label: 'Ön Yazı', placeholder: 'Kendinizi kısaca tanıtın...', width: '100%', sortOrder: 6 }
        ]
    },
    event: {
        name: 'Etkinlik Kaydı',
        icon: 'bi-calendar-event',
        color: '#ec4899',
        fields: [
            { fieldType: 19, label: 'Etkinlik Kayıt Formu', width: '100%', sortOrder: 0 },
            { fieldType: 1, label: 'Ad Soyad', isRequired: true, width: '50%', sortOrder: 1 },
            { fieldType: 4, label: 'E-posta', isRequired: true, width: '50%', sortOrder: 2 },
            { fieldType: 10, label: 'Etkinlik', options: 'Konferans\nWorkshop\nWebinar\nNetworking', isRequired: true, width: '50%', sortOrder: 3 },
            { fieldType: 6, label: 'Katılım Tarihi', isRequired: true, width: '50%', sortOrder: 4 },
            { fieldType: 12, label: 'Tercihler', options: 'Öğle yemeği istiyorum\nOtopark gerekiyor\nEngelli erişimi', width: '100%', sortOrder: 5 }
        ]
    },
    survey: {
        name: 'Müşteri Memnuniyeti',
        icon: 'bi-star-half',
        color: '#f59e0b',
        fields: [
            { fieldType: 19, label: 'Görüşleriniz Bizim İçin Değerli', width: '100%', sortOrder: 0 },
            { fieldType: 17, label: 'Genel Memnuniyet', isRequired: true, minValue: 1, maxValue: 5, defaultValue: '3', width: '100%', sortOrder: 1 },
            { fieldType: 11, label: 'Hizmeti tavsiye eder misiniz?', options: 'Evet\nHayır\nEmin değilim', isRequired: true, width: '100%', sortOrder: 2 },
            { fieldType: 2, label: 'Yorumlarınız', placeholder: 'Deneyiminizi paylaşın...', width: '100%', sortOrder: 3 }
        ]
    },
    registration: {
        name: 'Kayıt Formu',
        icon: 'bi-person-plus',
        color: '#10b981',
        fields: [
            { fieldType: 19, label: 'Kayıt Ol', width: '100%', sortOrder: 0 },
            { fieldType: 1, label: 'Kullanıcı Adı', isRequired: true, width: '50%', sortOrder: 1 },
            { fieldType: 4, label: 'E-posta', isRequired: true, width: '50%', sortOrder: 2 },
            { fieldType: 8, label: 'Şifre', isRequired: true, width: '50%', sortOrder: 3 },
            { fieldType: 8, label: 'Şifre Tekrar', isRequired: true, width: '50%', sortOrder: 4 },
            { fieldType: 12, label: 'Kullanım şartlarını kabul ediyorum', options: 'Kabul ediyorum', isRequired: true, width: '100%', sortOrder: 5 }
        ]
    }
};
